using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using SPMLoader.Model;

namespace SPMLoader.Behaviors
{
    public class ColumnHeaderBehavior : Behavior<DataGrid>
    {
        static readonly int ColumnWidth = 150;

        protected override void OnAttached()
        {            
            AssociatedObject.AutoGeneratingColumn += OnAutoGeneratingColumn;
            AssociatedObject.PreparingCellForEdit += OnPreparingCellForEdit;
            AssociatedObject.CellEditEnding += OnCellEditEnding;
            AssociatedObject.BeginningEdit += OnBeginningEdit;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= OnAutoGeneratingColumn;
            AssociatedObject.CellEditEnding -= OnCellEditEnding;
            AssociatedObject.PreparingCellForEdit += OnPreparingCellForEdit;
            AssociatedObject.BeginningEdit -= OnBeginningEdit;
        }

        protected void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }

        protected void OnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            //// хочу подменить TextCell на ComboBoxCell
            //DataGrid dataGrid = (DataGrid) sender;
            //var bind = (e.Column as DataGridBoundColumn)?.Binding as Binding;
            //var src = bind?.Source;
            //if (!(src is DummyDictPropValue))
            //    return;

            //e.EditingEventArgs.
        }

        protected void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
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

        //private void OnC(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex >= FIRST_COL && e.ColumnIndex <= LAST_COL && e.RowIndex == ROW_OF_INTEREST)
        //    {
        //        object value = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        //        dataGrid.Columns[e.ColumnIndex].CellTemplate = new DataGridViewComboBoxCell();
        //        var cell = new DataGridViewComboBoxCell { Value = value };
        //        cell.Items.AddRange(_values);
        //        dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] = cell;
        //    }
        //}

        //private void dataGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex >= FIRST_COL && e.ColumnIndex <= LAST_COL && e.RowIndex == ROW_OF_INTEREST)
        //    {
        //        object value = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        //        dataGrid.Columns[e.ColumnIndex].CellTemplate = new DataGridViewTextBoxCell();
        //        var cell = new DataGridViewTextBoxCell { Value = value };
        //        dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] = cell;
        //    }
        //}

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
