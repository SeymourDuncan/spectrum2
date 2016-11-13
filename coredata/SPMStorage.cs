using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

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

        public MySqlConnectionStringBuilder ConnectionString { get; private set; }

        /// <summary>
        /// проверяем подключение
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public bool Connect(string server, string user, string password, string database)
        {
            ConnectionString.Server = server;
            ConnectionString.UserID = user;
            ConnectionString.Password = password;
            ConnectionString.Database = database;
            var success = true;
            using (var conn = new MySqlConnection(ConnectionString.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    success = false;
                }
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
                LoadSystems(conn);
                LoadClasses(conn);
                LoadObjects(conn);
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
                            while (valReader.Read())
                            {
                                smpObj.Values.Clear();
                                smpObj.Values.Add(valReader.GetDouble("L_VAL"), valReader.GetDouble("K_VAL"));
                            }
                            valReader.Close();
                        }
                    }
                }
            }


        }

        public bool SaveOneObject(int sysId, int classId, string name, Dictionary<double, double> values,
            string comment = "")
        {
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
                var cStr = new StringBuilder(SqlHelper.InsertObjectQuery);
                var tmpLst = values.Select(val => $"('{id}','{val.Key}', '{val.Value}')").ToList();                
                cStr.Append(string.Join(", ", tmpLst));
                cStr.Append(";");

                using (var cmd = new MySqlCommand(cStr.ToString(), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                return true;
            }

        }
    }
}
