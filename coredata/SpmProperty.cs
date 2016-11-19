using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coredata
{   
    public class SpmPropertyValue
    {
        public SpmProperty Property;
        public string Value;
        public SpmObject Object;

        public dynamic GetValue()
        {
            dynamic res = Value;
            try
            {
                switch (Property.Type)
                {
                    case SpmTypeEnum.stDouble:
                        res = Convert.ToDouble(Value);
                        return res;
                    case SpmTypeEnum.stDate:
                        res = Convert.ToDateTime(Value);
                        return res;
                    case SpmTypeEnum.stInteger:
                        res = Convert.ToInt32(Value);
                        return res;
                    case SpmTypeEnum.stString:
                    case SpmTypeEnum.stUnknown:                       
                        return Value;
                    // если справочный тип, то значение является DictValue.Name
                    case SpmTypeEnum.stDictType:
                        var dict = Property.Dictionary;
                        res = dict.GetValue(Convert.ToInt32(Value)).Value;
                        return res;
                    default:
                        return Value;
                } 
            }
            catch (Exception e)
            {
                // Log
                return Value;
            }           
        }
    }
    
    public class SpmProperties
    {
        public List<SpmProperty> Properties = new List<SpmProperty>();

        public void Add(SpmProperty val)
        {
            Properties.Add(val);
        }
    }

    public class SpmProperty: SpmBase
    {
        public SpmProperty(int id, string name): base(id, name)
        {                  
        }        
        public SpmTypeEnum Type { get; set; }
        public SpmDictionary Dictionary { get; set; }
    }
}
