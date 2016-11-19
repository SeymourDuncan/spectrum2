using System.Collections.Generic;
using System.ComponentModel;
using Coredata;

namespace SPMLoader.Model
{
    /// <summary>
    /// Хранит значения которые отображаются в таблице "Свойства"
    /// </summary>
    public class DummyPropValue
    {
        SpmProperty _prop;
        protected bool _isDict;

        public DummyPropValue(SpmProperty prop)
        {
            _prop = prop;
            PropertyName = prop.Name;
            PropertyValue = "";
            _isDict = false;
        }

        public SpmProperty GetProperty()
        {
            return _prop;
        }
        
        public virtual string GetPropertyValue()
        {
            return PropertyValue;
        }

        [DisplayName(@"Свойство")]
        public string PropertyName { get; set; }
        /// <summary>
        /// Значение, которое показывается
        /// </summary>
        [DisplayName(@"Значение")]        
        public string PropertyValue { get; set; }

        public bool IsDict
        {
            get { return _isDict; }
            set { _isDict = value; }
        }
    }

    public class DummyDictPropValue : DummyPropValue
    {
        public DummyDictPropValue(SpmProperty prop) : base(prop)
        {
            _isDict = true;
            Dictionary = prop.Dictionary;            
        }        

        public DictValue SelectedItem { get; set; }
        public SpmDictionary Dictionary { get; set; }
        
        // Возвращает справочный Id в виде string-a.
        public override string GetPropertyValue()
        {
            return SelectedItem.Id.ToString();
        }
    }
}
