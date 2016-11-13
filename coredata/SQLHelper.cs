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

        public static string InsertObjectQuery = "Insert into OBJECT(NAME, CLASS_ID, SYSTEM_ID, COMMENT) values(@name, @class_id, @system_id, @comment)";
        public static string InsertSpmValuesQuery = "Insert into SPECTRUM_VALUE(OBJ_ID, L_VAL, K_VAL) values";
    }
}   
