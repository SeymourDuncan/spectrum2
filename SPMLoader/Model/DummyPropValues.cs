using System;
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

        public DummyPropValue(SpmProperty prop, string val = "")
        {
            _prop = prop;
            PropertyName = prop.Name;
            PropertyValue = val;
            if (string.IsNullOrEmpty(PropertyValue))
                PropertyValue = _prop.GetDefaultValue();
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

        public string PropertyName { get; set; }
        /// <summary>
        /// Значение, которое показывается
        /// </summary>
        
        public string PropertyValue { get; set; }

        public bool IsDict
        {
            get { return _isDict; }
            set { _isDict = value; }
        }
    }

    public class DummyDictPropValue : DummyPropValue
    {
        public DummyDictPropValue(SpmProperty prop, string val = "") : base(prop, val)
        {
            _isDict = true;
            Dictionary = prop.Dictionary;

            if (!string.IsNullOrEmpty(val))
            {
                int id = 0;
                if (Int32.TryParse(val, out id))
                {
                    SelectedItem = Dictionary.GetValue(id);
                }
            }                
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
