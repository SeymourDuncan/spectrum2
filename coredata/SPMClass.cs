using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coredata
{
    /// <summary>
    /// Для классов и подклассов элементов. Нужен только для дерева
    /// </summary>
    public class SpmClass : SpmBase, ISpmNode
    {
        public SpmClass(int id, string name, bool isRootClass, SpmSystem parentSystem) : base(id, name)
        {
            IsRootClass = isRootClass;
            ParentSystem = parentSystem;
            ChildList = new List<ISpmNode>();
        }
        // Нужен чтобы в системе не хранить 2 списка объектов.
        public bool IsRootClass { get; set; }
        /// <summary>
        /// Система к которой относится класс.
        /// </summary>
        public SpmSystem ParentSystem { get; set; }
        public IEnumerable<ISpmNode> GetChildNodes()
        {
            return ChildList;
        }

        public string GetName()
        {
            return Name;
        }

        public SpmNodeType GetNodeType()
        {
            return SpmNodeType.SntClass;
        }

        public IList<ISpmNode> ChildList;
    }
}
