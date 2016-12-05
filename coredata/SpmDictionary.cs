using System;
using System.Collections.Generic;
using System.Linq;

namespace Coredata
{
    public struct DictValue
    {
        // этот id уникален в пределах одного справочника,
        // тот есть этот id не из БД
        
        // 0 - отсутствие значения
        public int Id;
        public string Value;
        public string Comment;

        public override string ToString()
        {
            return Value;
        }
    }

    // Объект справочник
    public class SpmDictionary: SpmBase
    {        
        public SpmDictionary(int id, string name, string comment): base(id, name)
        {
            Values = new List<DictValue>();
            Comment = comment;
        }

        public List<DictValue> Values { get; set; } 
        public string Comment { get; set; }


        public void AddValue(DictValue dv)
        {
            Values.Add(dv);
        }

        public DictValue GetValue(int id)
        {
            return Values.FirstOrDefault(dv => dv.Id == id);
        }
    }

    /// <summary>
    /// Все справочники
    /// </summary>
    public class SpmDictionaries
    {
        public List<SpmDictionary> Values = new List<SpmDictionary>();
        public string Comment { get; set; }

        public void AddDictionary(SpmDictionary dict)
        {
            Values.Add(dict);
        }
        
        public SpmDictionary GetDictoinary(int type)
        {
            return Values.FirstOrDefault(dv => dv.Id == type);
        }
    }

}
