using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coredata
{
    public static class SqlHelper
    {
        public static string SelectSystemsQuery = "Select * from system";

        public static string SelectObjectsBySystemQuery = "Select * from object where system_id = @systemId";

        public static string SelectClassesBySystemQuery = "Select * from class where system_id = @systemId";

        public static string SelectSpmValuesByObjectQuery = "Select * from spectrum_value where obj_id = @objId order by l_val";

        public static string SelectDictionariesQuery = "Select * from dictionary";
        public static string SelectDictionaryValueQuery = "Select * from dictionary_value where dict_type = @dict_id order by id";

        public static string SelectPropertyBySystemQuery = "Select * from property where system_id = @system_id";
        public static string SelectPropertyValueQuery = "Select * from property_value where property_id = @property_id";

        public static string SelectPropValueTypesQuery = "Select * from value_type";

        public static string InsertObjectQuery = "Insert into object(name, class_id, system_id, comment) values(@name, @class_id, @system_id, @comment)";
        public static string InsertSpmValuesQuery = "Insert into spectrum_value(obj_id, l_val, k_val) values";
        public static string InsertPropValuesQuery = "Insert into property_value(obj_id, property_id, value) values";
        public static string InsertPropValueQuery = "Insert into property_value(obj_id, property_id, value) values (@objId, @propId, @value)";

        public static string UpdateObjectQuery = "Update object set name=@name, comment=@comment where id=@objId";
        public static string UpdatePropValuesQuery = "Update property_value set value=@value where obj_id=@objId and property_id=@propId";

        public static string DeleteSpmObjectQuery = "Delete from object where id=@Id";
        public static string DeleteSpmValuesQuery = "Delete from spectrum_value where obj_id=@objId";
        public static string DeleteSpmPropValuesQuery = "Delete from property_value where obj_id=@objId";
    }
}   
