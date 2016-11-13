using System;
using System.Collections.Generic;
using System.Linq;

namespace Coredata
{
    public class SpmSystem : SpmBase, ISpmNode
    {
        public SpmSystem(int id, string name): base(id, name)
        {
        }
        /// <summary>
        /// Список объектов спектров
        /// </summary>
        public IList<SpmObject> Objects { get; set; } = new List<SpmObject>();
        /// <summary>
        /// Список классов
        /// </summary>
        public IList<SpmClass> Classes { get; set; } = new List<SpmClass>();
        /// <summary>
        /// Список детей дерева. По сути тут всегда SPMClass-ы, но только те, которые в дереве напрямую связаны с системой
        /// </summary>
        
        public IEnumerable<ISpmNode> GetChildNodes()
        {
            return Classes.Where(cl => cl.IsRootClass);
        }

        public string GetName()
        {
            return Name;
        }

        public SpmNodeType GetNodeType()
        {
            return SpmNodeType.SntSystem;
        }
    }
}
