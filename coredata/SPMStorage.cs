using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using Spm.Shared;

namespace Coredata
{
    public class SpmStorage : ISpmLoader
    {
        public SpmStorage()
        {
            ConnectionString = new MySqlConnectionStringBuilder();
        }

        private IList<SpmSystem> _model = new List<SpmSystem>();
        private bool _isModelLoaded = false;

        public void Clear()
        {
            _isModelLoaded = false;
            _model.Clear();
        }
        
        // собственно модель - это список систем
        public IList<SpmSystem> Model
        {
            get
            {
                if (!_isModelLoaded)
                {
                    _isModelLoaded = InitModel();
                }
                return _model;
            }
        }

        public async Task<IList<SpmSystem>> GetModel()
        {
            if (!_isModelLoaded)
            {
                _isModelLoaded = await Task<bool>.Factory.StartNew(InitModel);
            }            
            return _model;
        }

        public MySqlConnectionStringBuilder ConnectionString { get; private set; }

        public SpmDictionaries Dictionaries { get; set; }

        public SpmPropValueTypes PropValueTypes { get; set; }

        public async Task<bool> Connect(ConnectionData cdata)
        {
            return await Connect(cdata.ServerName, cdata.UserName, cdata.Password, cdata.Database);
        }

        /// <summary>
        /// проверяем подключение
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public async Task<bool> Connect(string server, string user, string password, string database)
        {
            ConnectionString.Server = server;
            ConnectionString.UserID = user;
            ConnectionString.Password = password;
            ConnectionString.Database = database;
            var success = false;
            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        conn.Open();
                        success = true;
                    }
                    catch
                    {
                        success = false;
                    }
                });

            }
            return success;
        }

        /// <summary>
        /// Поднимает всю модель данных. Тут данных - фигня
        /// </summary>
        /// <returns></returns>
        public bool InitModel()
        {
            // и вот че? когда освободится?
            _model.Clear();
            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                conn.Open();
                // Служебная часть
                LoadPropValueTypes(conn);
                LoadDictionaries(conn);
                // Данные
                LoadSystems(conn);
                LoadClasses(conn);
                LoadObjects(conn);

                LoadProperties(conn);
            }
            return true;
        }

        public void LoadSystems(MySqlConnection conn)
        {
            using (var cmd = new MySqlCommand(SqlHelper.SelectSystemsQuery, conn))
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var spmSyst = new SpmSystem(reader.GetInt32("ID"), reader.GetString("NAME"));
                    _model.Add(spmSyst);
                }
                reader.Close();
            }
        }

        public void LoadClasses(MySqlConnection conn)
        {
            using (var cmd = new MySqlCommand(SqlHelper.SelectClassesBySystemQuery, conn))
            {
                cmd.Parameters.Add("@systemId", MySqlDbType.Int32);
                var tmp = new List<int>();
                foreach (var sys in _model)
                {
                    tmp.Clear();
                    cmd.Parameters["@systemId"].Value = sys.Id;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var spmClass = new SpmClass(reader.GetInt32("ID"), reader.GetString("NAME"),
                            reader.GetInt32("PARENT_ID") == 0, sys);
                        sys.Classes.Add(spmClass);
                        tmp.Add(reader.GetInt32("PARENT_ID"));
                    }
                    reader.Close();
                    // i - id класса, tmp[i] - id его родителя. Делаю так, чтобы не держать лишние данные в модели.
                    for (var i = 0; i < sys.Classes.Count; ++i)
                    {
                        // ищем родителя-класс
                        var parent = sys.Classes.FirstOrDefault(_cl => _cl.Id == tmp[i]);
                        parent?.ChildList.Add(sys.Classes[i]);
                    }
                }
            }
        }

        public void LoadObjects(MySqlConnection conn)
        {
            using (var cmd = new MySqlCommand(SqlHelper.SelectObjectsBySystemQuery, conn))
            {
                cmd.Parameters.Add("@systemId", MySqlDbType.Int32);
                foreach (var sys in _model)
                {
                    cmd.Parameters["@systemId"].Value = sys.Id;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var smpObj = new SpmObject(reader.GetInt32("ID"), reader.GetString("NAME"), sys,
                            reader.GetString("COMMENT"));
                        sys.Objects.Add(smpObj);
                        // двусторонняя связь класс-объект
                        var cls = sys.Classes.FirstOrDefault(cl => cl.Id == reader.GetInt32("CLASS_ID"));
                        cls?.ChildList.Add(smpObj);
                        smpObj.Class = cls;
                    }
                    reader.Close();

                    // тут же сразу и значения прочитаю
                    foreach (var smpObj in sys.Objects)
                    {
                        using (var cmd2 = new MySqlCommand(SqlHelper.SelectSpmValuesByObjectQuery, conn))
                        {
                            cmd2.Parameters.AddWithValue("@objId", smpObj.Id);
                            var valReader = cmd2.ExecuteReader();
                            smpObj.Values.Clear();
                            while (valReader.Read())
                            {
                                smpObj.Values.Add(new SpmLKValue() {Kval = valReader.GetDouble("K_VAL"), Lval = valReader.GetDouble("L_VAL")});
                            }
                            valReader.Close();
                        }
                    }
                }
            }
        }

        public void LoadProperties(MySqlConnection conn)
        {
            foreach (var sys in _model)
            {
                // Загружаем свойства
                using (var cmd = new MySqlCommand(SqlHelper.SelectPropertyBySystemQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@system_id", sys.Id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var spmProp = new SpmProperty(reader.GetInt32("ID"), reader.GetString("NAME"));

                        sys.Properties.Add(spmProp);
                        spmProp.Type = PropValueTypes.GetTypeById(reader.GetInt32("VALUE_TYPE_ID"));

                        var ord = reader.GetOrdinal("DICTIONARY");
                        if (!reader.IsDBNull(ord))
                        {
                            var dict = Dictionaries.GetDictoinary(reader.GetInt32(ord));
                            if (spmProp.Type == SpmTypeEnum.stDictType && dict != null)
                            {
                                spmProp.Dictionary = dict;
                            }
                        }                       
                    }
                    reader.Close();
                }

                // Загружаем значения свойств
                using (var cmd2 = new MySqlCommand(SqlHelper.SelectPropertyValueQuery, conn))
                {
                    cmd2.Parameters.AddWithValue("@property_id", MySqlDbType.Int32);
                    foreach (var prop in sys.Properties.Properties)
                    {
                        cmd2.Parameters["@property_id"].Value = prop.Id;
                        var reader2 = cmd2.ExecuteReader();
                        while (reader2.Read())
                        {
                            var objId = reader2.GetInt32("OBJ_ID");
                            var obj = sys.Objects.FirstOrDefault(o => o.Id == objId);
                            if (obj == null)
                                continue;
                            var propVal = new SpmPropertyValue
                            {
                                Property = prop,
                                Value = reader2.GetString("VALUE"),
                                Object = obj
                            };
                            sys.AddPropertyValue(propVal);
                        }
                        reader2.Close();
                    }                                      
                }
            }

        }

        public void LoadDictionaries(MySqlConnection conn)
        {
            Dictionaries = new SpmDictionaries();
            using (var cmd = new MySqlCommand(SqlHelper.SelectDictionariesQuery, conn))
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var spmDict = new SpmDictionary(reader.GetInt32("ID"), reader.GetString("NAME"),
                        reader.GetString("COMMENT"));
                    Dictionaries.AddDictionary(spmDict);
                }
                reader.Close();

                foreach (var dict in Dictionaries.Values)
                {
                    LoadDictionary(conn, dict);
                }
            }
        }

        public void LoadDictionary(MySqlConnection conn, SpmDictionary spmDict)
        {
            using (var cmd = new MySqlCommand(SqlHelper.SelectDictionaryValueQuery, conn))
            {
                cmd.Parameters.AddWithValue("@dict_id", spmDict.Id);
                var reader = cmd.ExecuteReader();
                int idx = 1; // строго нах! потому что 0 - это дефолтный id. Он используется при проверке на валидность
                while (reader.Read())
                {
                    var dictVal = new DictValue();
                    //dictVal.Id = idx;
                    dictVal.Id = reader.GetInt32("ID");
                    dictVal.Value = reader.GetString("NAME");
                    var ord = reader.GetOrdinal("COMMENT");
                    dictVal.Comment = reader.IsDBNull(ord) ? "" : reader.GetString(ord);

                    spmDict.AddValue(dictVal);
                    idx++;
                }
                reader.Close();
            }
        }

        public void LoadPropValueTypes(MySqlConnection conn)
        {
            PropValueTypes = new SpmPropValueTypes();
            using (var cmd = new MySqlCommand(SqlHelper.SelectPropValueTypesQuery, conn))
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PropValueTypes.AddValue(reader.GetInt32("ID"), reader.GetString("NAME"));
                }
                reader.Close();
            }
        }

        public bool SaveObjToDb(SpmObject spmObj)
        {
            var classId = spmObj.Class.Id;
            var sysId = spmObj.System.Id;
            var name = spmObj.Name;
            var values = spmObj.Values;
            var comment = spmObj.Comment;

            if (sysId == 0 || classId == 0 || values.Count == 0 || string.IsNullOrEmpty(name))
                return false;

            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                conn.Open();
                // добавляем объект
                int id = 0;
                using (var cmd = new MySqlCommand(SqlHelper.InsertObjectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@class_id", classId);
                    cmd.Parameters.AddWithValue("@system_id", sysId);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    cmd.ExecuteNonQuery();
                    id = (int) cmd.LastInsertedId;
                }

                // добавляем значения
                var cStr = new StringBuilder(SqlHelper.InsertSpmValuesQuery);

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                var tmpLst =
                    values.Select(val => $"({id}, {val.Lval.ToString(nfi)}, {val.Kval.ToString(nfi)})").ToList();
                cStr.Append(string.Join(", ", tmpLst));
                cStr.Append(";");


                using (var cmd = new MySqlCommand(cStr.ToString(), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                // добавляем объект в модель
                spmObj.Id = id;
                spmObj.System.Objects.Add(spmObj);
                spmObj.Class.ChildList.Add(spmObj);
                return true;
            }

        }

        public bool SavePropValsToDb(List<SpmPropertyValue> propVals)
        {
            if (!propVals.Any())
                return false;
            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                conn.Open();
                // добавляем значения
                var cStr = new StringBuilder(SqlHelper.InsertPropValuesQuery);                

                var tmpLst =
                    propVals.Select(val => $"({val.Object.Id}, {val.Property.Id}, \"{val.Value}\")").ToList();
                cStr.Append(string.Join(", ", tmpLst));
                cStr.Append(";");

                using (var cmd = new MySqlCommand(cStr.ToString(), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                // добавляем свойства
                propVals.ForEach(val =>
                {
                    var sys = propVals[0].Object.System;
                    sys.AddPropertyValue(val);
                });
                
                return true;
            }
        }

        public bool UpdateObjToDb(SpmObject spmObj, string name, string comment, IList<SpmLKValue> values)
        {                       
            if (values.Count == 0 || string.IsNullOrEmpty(name))
                return false;

            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                conn.Open();
                // обновляем имя и коммент
                using (var cmd = new MySqlCommand(SqlHelper.UpdateObjectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@objId", spmObj.Id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    cmd.ExecuteNonQuery();
                }
                // удаляем старые значения
                using (var cmd = new MySqlCommand(SqlHelper.DeleteSpmValuesQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@objId", spmObj.Id);
                    cmd.ExecuteNonQuery();                    
                }
                // добавляем значения
                var cStr = new StringBuilder(SqlHelper.InsertSpmValuesQuery);

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                var tmpLst =
                    values.Select(val => $"({spmObj.Id}, {val.Lval.ToString(nfi)}, {val.Kval.ToString(nfi)})").ToList();
                cStr.Append(string.Join(", ", tmpLst));
                cStr.Append(";");
                using (var cmd = new MySqlCommand(cStr.ToString(), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                // добавляем объект в модель                              
                return true;
            }

        }

        public bool UpdatePropValsToDb(List<SpmPropertyValue> propVals)
        {
            // у любой системы есть свойства
            if (!propVals.Any())
                return false;
            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                conn.Open();

                // обновляем свойства в модели
                var sys = propVals[0].Object.System;
                propVals.ForEach(val =>
                {
                    var propVal = sys.PropValues.FirstOrDefault(pval => pval.Object.Id == val.Object.Id && pval.Property.Id == val.Property.Id);                                       
                    if (propVal == null)
                    {
                        using (var cmd = new MySqlCommand(SqlHelper.InsertPropValueQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@objId", val.Object.Id);
                            cmd.Parameters.AddWithValue("@propId", val.Property.Id);
                            cmd.Parameters.AddWithValue("@value", val.Value);
                            cmd.ExecuteNonQuery();
                        }
                        sys.PropValues.Add(val);
                    }
                    else
                    {
                        using (var cmd = new MySqlCommand(SqlHelper.UpdatePropValuesQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@objId", val.Object.Id);
                            cmd.Parameters.AddWithValue("@propId", val.Property.Id);
                            cmd.Parameters.AddWithValue("@value", val.Value);
                            cmd.ExecuteNonQuery();
                        }
                        propVal.Value = val.Value;
                    }
                });

                return true;
            }
        }

        public bool DeleteObjFromDb(SpmObject obj)
        {
            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(SqlHelper.DeleteSpmObjectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", obj.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            // выпиливаем сами себя
            obj.System.Objects.Remove(obj);
            obj.Class.ChildList.Remove(obj);            
            return true;
        }

    }
}
