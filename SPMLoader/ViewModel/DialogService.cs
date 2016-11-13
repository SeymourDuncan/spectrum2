using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace SPMLoader.ViewModel
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title);
        void ShowError(string errorMessage);        
        string GetOpenFileDialogResult();
    }

    class DialogService : IDialogService
    {
        public string GetOpenFileDialogResult()
        {
            var res = "";
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "CSV файл(.csv)|*.csv|Файл Excel (*.xlsx)|*.xlsx";
            fileDialog.FilterIndex = 0;
            var dialogRes = fileDialog.ShowDialog();
            if (dialogRes.HasValue && dialogRes.Value)
            {
                res = fileDialog.FileName;
            }
            return res;
        }

        public void ShowError(string errorMessage)
        {            
            var dialog = MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowMessage(string message, string title)
        {
            var dialog = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
