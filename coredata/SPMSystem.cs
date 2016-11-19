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
               
        ///<summary>
        /// Список возможных пропертей системы.
        /// </summary>
        public SpmProperties Properties { get; set; } = new SpmProperties();
        /// <summary>
        /// Список значений пропертей.
        /// </summary>
        /// <remarks>
        /// Сделано так, потому что наличие определенного свойства у объекта зависит от системы.
        /// Теоретически можно утолкать список SpmPropertyValue в SpmObject, но мне кажется это будет не очень круто.
        /// Для прозрачности сделаю метод расширения у SpmObject который выдает значение запрашиваемого свойства.
        /// </remarks>
        public List<SpmPropertyValue> PropValues { get; set; } = new List<SpmPropertyValue>();

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

        public void AddPropertyValue(SpmPropertyValue val)
        {
            // проверим есть ли у системы вообще такая проперть
            var prop = val.Property;
            if (!Properties.Properties.Contains(prop))
                return; //  throw?
            PropValues.Add(val);
        }        
    }
}
