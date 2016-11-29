using System;
using System.Collections.Generic;

namespace Coredata
{
    public struct SpmLKValue
    {
        public double Lval;
        public double Kval;
    }

    public class SpmObject : SpmBase, ISpmNode
    {
        // объекту пофигу какого он класса. Думаю не понадобится
        public SpmObject(int id, string name, SpmSystem system, string comment) : base(id, name)
        {
            System = system;                        
            Comment = comment;
        }
        public SpmSystem System { get; set; }
        public SpmClass Class { get; set; }
        // TODO гавно какое-то.  переделать под List<Point> или вроде того
        public List<SpmLKValue> Values { get; set; } = new List<SpmLKValue>();
        public string Comment { get; set; }

        // тут null потому что у объектов не может быть детей
        public IEnumerable<ISpmNode> GetChildNodes()
        {
            return null;
        }

        public SpmNodeType GetNodeType()
        {
            return SpmNodeType.SntObject;
        }

        public string GetName()
        {
            return Name;
        }        
    }
}
