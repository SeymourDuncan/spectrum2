using System;
using System.Collections.Generic;

namespace Coredata
{
    public class SpmObject : SpmBase, ISpmNode
    {
        // объекту пофигу какого он класса. Думаю не понадобится
        public SpmObject(int id, string name, SpmSystem system, string comment) : base(id, name)
        {
            System = system;            
            Comment = Comment;
        }
        public SpmSystem System { get; set; }
        public SpmClass Class { get; set; }
        public Dictionary<double, double> Values { get; set; } = new Dictionary<double, double>();
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
