using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SPMLoader.Behaviors
{
    public class ColumnHeaderBehavior : Behavior<DataGrid>
    {
        static readonly int ColumnWidth = 150;

        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn +=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AssociatedObject.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(OnCellEditEnding);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AssociatedObject.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(OnCellEditEnding);
        }

        protected void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }

        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string displayName = GetPropertyDisplayName(e.PropertyDescriptor);
            e.Column.Width = ColumnWidth;
            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }
            else
            {
                e.Cancel = true;
            }            
            
        }

        protected static string GetPropertyDisplayName(object descriptor)
        {
            PropertyDescriptor pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                DisplayNameAttribute attr = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                if ((attr != null) && (attr != DisplayNameAttribute.Default))
                {
                    return attr.DisplayName;
                }
            }
            else
            {
                PropertyInfo pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    Object[] attrs = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    foreach (var att in attrs)
                    {
                        DisplayNameAttribute attribute = att as DisplayNameAttribute;
                        if ((attribute != null) && (attribute != DisplayNameAttribute.Default))
                        {
                            return attribute.DisplayName;
                        }
                    }
                }
            }
            return null;
        }
    }
}
