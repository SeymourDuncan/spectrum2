using System;
using System.Collections.Generic;
using System.Linq;

namespace Coredata
{
    /// <summary>
    /// Рарезервированные типы спавочников
    /// </summary>
    /// <remarks>
    /// Теоретически может быть множество справочников. Но есть те, которые непосредственно используются в коде.
    /// Эти справочники будут иметь тип отличный от sdtUnknown;
    /// sdt - spetrum dict type
    /// </remarks>
    public enum SpmDictionaryType
    {
        sdtUnknown = 0,
        sdtDisease = 1
    }

    public struct DictValue
    {
        // этот id уникален в пределах одного справочника,
        // тот есть этот id не из БД
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
        public SpmDictionaryType Type { get; set; }
        public SpmDictionary(int id, string name, string comment): base(id, name)
        {
            if (!Enum.IsDefined(typeof (SpmDictionaryType), id))
            {
                Type = (SpmDictionaryType)id;
            }
            else
            {
                Type = SpmDictionaryType.sdtUnknown;
            }
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
            return Values.First(dv => dv.Id == id);
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
        public SpmDictionary GetDictoinary(int id)
        {
            return Values.FirstOrDefault(dv => dv.Id == id);
        }
        public SpmDictionary GetDictoinary(SpmDictionaryType type)
        {
            return Values.FirstOrDefault(dv => dv.Type == type);
        }
    }

}
