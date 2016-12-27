using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Coredata;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Spm.Shared;
using GalaSoft.MvvmLight.Messaging;
using spectrum2.Model;

namespace Spectrum2.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICommand _openConnectionSettingCommand;
        private IList<ISpmNode> _rootNodes;
        private ISpmNode _selectedNode;
        private readonly ICommand _loadedCommand;

        public ConnectionData ConnData { get; set; }
        public IDialogService DiagService { get; set; }

        public MainViewModel()
        {
            // ������ ���������            
            ConnData = new ConnectionData(Properties.Settings.Default.Host, Properties.Settings.Default.User, Properties.Settings.Default.Password, Properties.Settings.Default.DataBase);
            // ������� ���������
            DataModel = new SpmStorage();
        }

        public ICommand OpenConnectionSettingCommand
        {
            get
            {
                return _openConnectionSettingCommand ?? new RelayCommand(() =>
                {
                    Messenger.Default.Send(new ShowDialogMessage(DialogWindowTypes.DwConnectionSettings));
                });
            }
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand ?? new RelayCommand(()=> { DoOnLoaded(); }); }
        }

        public void OnConnDataChanged(object sender, ConnectionData cdata)
        {
            ConnData = cdata;
            SaveConnData();
            Connect();
        }

        // ����� View ��� ���������
        void DoOnLoaded()
        {
            Connect();
        }

        // ��������� ���
        void SaveConnData()
        {
            Properties.Settings.Default.Host = ConnData.ServerName;
            Properties.Settings.Default.User = ConnData.UserName;
            Properties.Settings.Default.DataBase = ConnData.Database;
            Properties.Settings.Default.Password = ConnData.Password;  
            Properties.Settings.Default.Save();          
        }

        // ������

        // ������
        public IList<ISpmNode> RootNodes
        {
            get { return _rootNodes; }
            set
            {
                _rootNodes = value;
                RaisePropertyChanged(() => RootNodes);
            }
        }

        //������
        public SpmStorage DataModel { get; set; }

        
        void Connect()
        {
            if (DataModel.Connect(ConnData))
            {                
                try
                {
                    DataModel.Clear();
                    RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                }
                catch (Exception e)
                {
                    //Status = $"���������� ������ �������� �� �������. �������: {e.Message}";
                    return;
                }
                //Status = "������ �������� ���������.";
            }
            //else
            //{
            //    Status = "�� ������� ��������� ����������� � ��.";
            //}
        }

        public ISpmNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                RaisePropertyChanged();
            }
        }
    }
}