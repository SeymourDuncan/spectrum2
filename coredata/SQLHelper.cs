using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coredata
{
    public static class SqlHelper
    {
        public static string SelectSystemsQuery = "Select * from SYSTEM";

        public static string SelectObjectsBySystemQuery = "Select * from OBJECT where SYSTEM_ID = @systemId";

        public static string SelectClassesBySystemQuery = "Select * from CLASS where SYSTEM_ID = @systemId";

        public static string SelectSpmValuesByObjectQuery = "Select * from SPECTRUM_VALUE where OBJ_ID = @objId order by L_VAL";

        public static string SelectDictionariesQuery = "Select * from DICTIONARY";
        public static string SelectDictionaryValueQuery = "Select * from DICTIONARY_VALUE where DICT_TYPE = @dict_id";

        public static string SelectPropertyBySystemQuery = "Select * from PROPERTY where SYSTEM_ID = @system_id";
        public static string SelectPropertyValueQuery = "Select * from PROPERTY_VALUE where PROPERTY_ID = @property_id";

        public static string SelectPropValueTypesQuery = "Select * from VALUE_TYPE";

        public static string InsertObjectQuery = "Insert into OBJECT(NAME, CLASS_ID, SYSTEM_ID, COMMENT) values(@name, @class_id, @system_id, @comment)";
        public static string InsertSpmValuesQuery = "Insert into SPECTRUM_VALUE(OBJ_ID, L_VAL, K_VAL) values";
        public static string InsertPropValuesQuery = "Insert into PROPERTY_VALUE(OBJ_ID, PROPERTY_ID, VALUE) values";
    }
}   
