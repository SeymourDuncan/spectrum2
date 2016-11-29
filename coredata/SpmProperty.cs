using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Coredata
{   
    public class SpmPropertyValue
    {
        public SpmProperty Property;

        public string Value
        {
            get { return _value; }
            set
            {
                if (Property.Validate(value))
                {
                    _value = value;
                }
                else
                {
                    _value = Property.GetDefaultValue();
                }
            }
        }        
        public SpmObject Object;
        private string _value = "";
         
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

        public string GetDefaultValue()
        {
            switch (Type)
            {                
                case SpmTypeEnum.stDouble:
                    return "0,0";
                case SpmTypeEnum.stInteger:
                    return "0";
                case SpmTypeEnum.stDictType:
                    return "0";                        
                default:
                    return "";
            }
        }
        public bool Validate(string val)
        {
            try
            {
                switch (Type)
                {
                    case SpmTypeEnum.stDate:
                        Convert.ToDateTime(val);
                        break;
                    case SpmTypeEnum.stDouble:
                        Convert.ToDouble(val);
                        break;
                    case SpmTypeEnum.stInteger:
                        Convert.ToInt32(val);
                        break;
                    case SpmTypeEnum.stDictType:
                        return Dictionary.GetValue(Convert.ToInt32(val)).Id != 0; // я - говнокодер! ура                        
                    default:
                        return true;                        
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
