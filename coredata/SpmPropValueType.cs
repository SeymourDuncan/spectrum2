using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coredata;

namespace Coredata
{
    public enum SpmTypeEnum
    {
        stUnknown = 0,
        stString,
        stInteger,
        stDouble,
        stDate,
        stDictType
    }

    public class SpmPropValueType
    {       
        public int Id;
        public SpmTypeEnum Type;
    }
    public class SpmPropValueTypes
    {
        private List<SpmPropValueType> Types;
        public SpmPropValueTypes()
        {
            Types = new List<SpmPropValueType>();
        }

        /// <summary>
        /// Регистрация типов
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void AddValue(int id, string name)
        {
            SpmTypeEnum stype;
            if (string.Equals(name, "varchar"))
            {
                stype = SpmTypeEnum.stString;
            }
            else if(string.Equals(name, "integer"))
            {
                stype = SpmTypeEnum.stInteger;
            }            
            else if (string.Equals(name, "double"))
            {
                stype = SpmTypeEnum.stDouble;
            }
            else if (string.Equals(name, "date"))
            {
                stype = SpmTypeEnum.stDate;
            }
            else if (string.Equals(name, "dictionary_type"))
            {
                stype = SpmTypeEnum.stDictType;
            }
            else
            {
                stype = SpmTypeEnum.stUnknown;
            }           
            Types.Add(new SpmPropValueType() { Id = id, Type = stype });
        }

        public SpmTypeEnum GetTypeById(int id)
        {
            var smpType = Types.FirstOrDefault(obj => obj.Id == id);
            if (smpType != null)
            {
                return smpType.Type;
            }
            return SpmTypeEnum.stUnknown;
        }

        public dynamic GetValueOf(string value, int typeId, SpmDictionary dict = null)
        {
            SpmTypeEnum type = GetTypeById(typeId);
            switch (type)
            {
                case SpmTypeEnum.stString:
                    return value;
                case SpmTypeEnum.stDate:
                    return Convert.ToDateTime(value);
                case SpmTypeEnum.stUnknown:
                    return value;
                case SpmTypeEnum.stDouble:
                    return Convert.ToDouble(value);
                case SpmTypeEnum.stInteger:
                    return Convert.ToInt32(value);
                case SpmTypeEnum.stDictType:
                    return dict?.GetValue(Convert.ToInt32(value));
                default:
                    return value;
            }                                    
        }
    }
}
