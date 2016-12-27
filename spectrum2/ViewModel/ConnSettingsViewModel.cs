using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using spectrum2.Model;

namespace spectrum2.ViewModel
{
    public class ConnSettingsViewModel: ViewModelBase
    {
        private ICommand _okClickCommand;
        private string _serverName;
        private string _userName;
        private string _password;
        private string _database;

        public ConnectionData ConnData { get; set; }

        public string ServerName
        {
            get { return _serverName; }
            set
            {
                _serverName = value;
                RaisePropertyChanged();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }

        public string Database
        {
            get { return _database; }
            set
            {
                _database = value;
                RaisePropertyChanged();
            }
        }


        public Action CloseDialogAction { get; set; }
       
        public ConnSettingsViewModel()
        {            
        }

        public void Clear()
        {
            ChangeConnData = null;
        }
        public void SetConnData(ConnectionData cdata)
        {
            ServerName = cdata.ServerName;
            Database = cdata.Database;
            UserName = cdata.UserName;
            Password = cdata.Password;
        }

        public event EventHandler<ConnectionData> ChangeConnData;

        public ICommand OkClickCommand
        {
            get { return _okClickCommand ?? new RelayCommand(() =>
            {
                ChangeConnData?.Invoke(this, new ConnectionData(ServerName, UserName, Password, Database));
                CloseDialogAction();
            }); }
        }
    }
}
