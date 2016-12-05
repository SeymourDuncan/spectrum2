using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Coredata;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using coredata;
using GalaSoft.MvvmLight.Command;
using SPMLoader.Model;

namespace SPMLoader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ICommand _connectCommand;
        private IList<ISpmNode> _rootNodes;
        private string _status;
        private ISpmNode _selectedNode;
        private SpmClass _selectedClass;
        private SpmSystem _selectedSystem;
        private ICommand _selectFileCommand;
        private string _fileName;
        private string _objectName;
        private bool _isSetAsFileName;
        private ObservableCollection<SpmDataGridItem> _spectrumValues;
        private ObservableCollection<DummyPropValue> _propertyValues;        
        private ICommand _deleteSelectedTableItem;
        private ICommand _addNewTableItemCommand;
        private ICommand _clearTableItemsCommand;
        private ICommand _loadedCommand;
        private string _password;
        private SpmObject _selectedObject;

        public MainViewModel()
        {            
            DataModel = new SpmStorage();            
            _spectrumValues = new ObservableCollection<SpmDataGridItem>();
            _propertyValues = new ObservableCollection<DummyPropValue>();
            ExecuteCommand = new AutoRelayCommand(DoExecute, CanDoExecute);
            EditCommand = new AutoRelayCommand(DoUpdate, CanDoUpdate);
            DeleteCommand = new AutoRelayCommand(DoDelete, CanDoUpdate);

            ExecuteCommand.DependsOn(() => SpectrumValues);
            ExecuteCommand.DependsOn(() => ObjectName);
            ExecuteCommand.DependsOn(() => SelectedClass);
            ExecuteCommand.DependsOn(() => SelectedSystem);

            EditCommand.DependsOn(() => SpectrumValues);
            EditCommand.DependsOn(() => ObjectName);
            EditCommand.DependsOn(() => SelectedClass);
            EditCommand.DependsOn(() => SelectedSystem);
            EditCommand.DependsOn(() => SelectedObject);

            DeleteCommand.DependsOn(() => SpectrumValues);
            DeleteCommand.DependsOn(() => ObjectName);
            DeleteCommand.DependsOn(() => SelectedClass);
            DeleteCommand.DependsOn(() => SelectedSystem);
            DeleteCommand.DependsOn(() => SelectedObject);
        }

        SpmStorage DataModel { get; set; }        
        public IDialogService DialogService { get; set; }

        public string ServerName { get; set; } = "localhost";
        public string UserName { get; set; } = "root";

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public string Database { get; set; } = "spectrum2";

        public string Comment { get; set; } = "";
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
    }

        public string SaveButtonCaption { get; set; } = "��������� ���������";
        public IList<ISpmNode> RootNodes
        {
            get { return _rootNodes; }
            set
            {
                _rootNodes = value; 
                RaisePropertyChanged(() => RootNodes);
            }
        }

        public ISpmNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                UpdateSelectedNodes();
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        public string ObjectName
        {
            get { return _objectName; }
            set
            {                
                this.Set(ref _objectName, value, broadcast: true);                
            }
        }

        public bool IsSetAsFileName
        {
            get
            {
                return _isSetAsFileName;
            }
            set
            {
                _isSetAsFileName = value;
                if (_isSetAsFileName)
                {
                    ObjectName = Path.GetFileNameWithoutExtension(FileName);
                }
            }
        }

        public SpmDataGridItem SelectedTableItem { get; set; }

        public ObservableCollection<SpmDataGridItem> SpectrumValues
        {
            get { return _spectrumValues; }
            set
            {
                // ��� ���������� ��������� ����������
                this.Set(ref _spectrumValues, value, broadcast: true);
            }
        }

        public ObservableCollection<DummyPropValue> PropertyValues
        {
            get { return _propertyValues; }
            set
            {
                _propertyValues = value;
                RaisePropertyChanged(() => PropertyValues);
            }
        }

        public SpmSystem SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                this.Set(ref _selectedSystem, value, broadcast: true);
                if (_selectedSystem != null)
                    UpdatePropertyValues();
            }
        }
        public SpmClass SelectedClass
        {
            get { return _selectedClass; }
            set
            {
                this.Set(ref _selectedClass, value, broadcast: true);
            }
        }
                
        public SpmObject SelectedObject
        {
            get { return _selectedObject; }
            set { this.Set(ref _selectedObject, value, broadcast: true); }
        }

        /// <summary>
        /// ��������� �������� � ������� ��������
        /// </summary>
        void UpdatePropertyValues()
        {
            PropertyValues.Clear();
            foreach (var prop in SelectedSystem.Properties.Properties)
            {
                string val = "";
                if (SelectedObject != null)
                    val = SelectedObject.GetPropValue(prop);
                var dummyProp = prop.Type == SpmTypeEnum.stDictType ? new DummyDictPropValue(prop, val) : new DummyPropValue(prop, val);
                PropertyValues.Add(dummyProp);
            }           
        }

        /// <summary>
        /// ������� ������� � ����� ���������� ��������
        /// </summary>
        void UpdateSelectedNodes()
        {                       
            switch (SelectedNode.GetNodeType())
            {
                case SpmNodeType.SntSystem:
                {
                    SelectedSystem = (SpmSystem) SelectedNode;
                    SelectedClass = null;
                    SelectedObject = null;
                    break;
                }
                case SpmNodeType.SntClass:
                {
                    var cl = (SpmClass) SelectedNode;
                    SelectedClass = cl;
                    if (cl?.ParentSystem.Id != SelectedSystem?.Id)
                        SelectedSystem = cl?.ParentSystem;
                    SelectedObject = null;
                    break;
                }
                case SpmNodeType.SntObject:
                {
                    SelectedObject = (SpmObject) SelectedNode;
                    if (SelectedObject != null)
                    {
                        ObjectName = SelectedObject.Name;
                        Comment = SelectedObject.Comment;
                        SpectrumValues.Clear();
                        foreach (var val in SelectedObject.Values)
                        {
                            SpectrumValues.Add(new SpmDataGridItem() {KValue = val.Kval, LValue = val.Lval});
                        }

                        SelectedSystem = SelectedObject.System;
                        SelectedClass = SelectedObject.Class;
                    }
                    break;
                }
            }
        }

        public ICommand ConnectCommand => _connectCommand?? (_connectCommand = new RelayCommand<object>((obj) =>
        {
            if (DataModel.Connect(ServerName, UserName, Password, Database))
            {
                Status = "����������� ���������. ����������� ������ ��������.";
                try
                {
                    DataModel.Clear();
                    RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                }
                catch (Exception e)
                {
                    Status = $"���������� ������ �������� �� �������. �������: {e.Message}";
                    return;
                }
                Status = "������ �������� ���������.";
            }
            else
            {
                Status = "�� ������� ��������� ����������� � ��.";
            }                                      
        }));

        public ICommand SelectFileCommand
        {
            get
            {
                return _selectFileCommand ?? (_selectFileCommand = new RelayCommand<object>((obj) =>
                {
                    FileName = DialogService.GetOpenFileDialogResult();
                    SpectrumValues.Clear();
                    if (!string.IsNullOrEmpty(FileName))
                    {
                        // ������� ����������                       
                        using (var parser = new FileParser())
                        {
                            if (parser.ParseFile(FileName))
                            {
                                parser.RowDoubleValues.ForEach(arr =>
                                {
                                    SpectrumValues.Add(new SpmDataGridItem() { LValue = arr[0], KValue = arr[1] });
                                });
                                ExecuteCommand.RaiseCanExecuteChanged();
                                EditCommand.RaiseCanExecuteChanged();
                            }
                        }
                    }
                }));
            }
        }

        public ICommand DeleteSelectedTableItemCommand
        {
            get
            {
                return _deleteSelectedTableItem ?? (_deleteSelectedTableItem = new RelayCommand<object>((obj) =>
                {
                    SpectrumValues.Remove(SelectedTableItem);
                    ExecuteCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                }));
            }
        }

        public ICommand AddNewTableItemCommand
        {
            get
            {
                return _addNewTableItemCommand ?? (_addNewTableItemCommand = new RelayCommand<object>((obj) =>
                {
                    SpectrumValues.Add(new SpmDataGridItem() {KValue = 0.0, LValue = 0.0 });
                    ExecuteCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                }));
            }
        }        

        public ICommand ClearTableItemsCommand
        {
            get
            {
                return _clearTableItemsCommand ?? (_clearTableItemsCommand = new RelayCommand<object>((obj) =>
                {
                    SpectrumValues.Clear();
                    ExecuteCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                }));
            }
        }

        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand<object>((obj) =>
                {
                    Password = "r2d2sat61kaz";
                }));
            }
        }
        // �������� ������ �������
        public AutoRelayCommand ExecuteCommand { get; set; }

        // ������������� ������
        public AutoRelayCommand EditCommand { get; set; }

        // ������������� ������
        public AutoRelayCommand DeleteCommand { get; set; }

        bool CanDoExecute()
        {
            if (string.IsNullOrEmpty(SelectedSystem?.Name) || (string.IsNullOrEmpty(SelectedClass?.Name)))
                return false;

            if (string.IsNullOrEmpty(ObjectName))
                return false;

            if (SpectrumValues?.Count < 1)
                return false;

            return true;
        }

        void DoExecute()
        {
            try
            {
                var spmObj = new SpmObject(0, ObjectName, SelectedSystem, Comment);
                spmObj.Class = SelectedClass;
                spmObj.Values = SpectrumValues.Select(item => new SpmLKValue() {Kval = item.KValue, Lval = item.LValue }).ToList();

                // ��������� ������
                if (DataModel.SaveObjToDb(spmObj))
                {
                    // �������� ������
                    RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                    // ��������� ��������     
                    var propValues = PropertyValues.Select(propv => new SpmPropertyValue()
                    {
                        Object = spmObj,
                        Property = propv.GetProperty(),
                        Value = propv.GetPropertyValue()
                    }).ToList();

                    if (DataModel.SavePropValsToDb(propValues))
                    {
                        // �������� ������
                        RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                    }
                    else
                    {
                        DialogService.ShowMessage(
                            $"������ �������� ������� ������� {spmObj.Name} � ��. ��.��� ��� ������������.");
                        return;
                    }

                    DialogService.ShowMessage($"������ {spmObj.Name} ������� �������� � ��");
                }
                else
                {
                    DialogService.ShowMessage($"������ �������� ������� {spmObj.Name} � ��. ��.��� ��� ������������.");
                }
            }
            catch (Exception e)
            {
                DialogService.ShowMessage($"������ ���������� ������� {SelectedObject.Name}. ����� ������: {e.Message}");
            }        
    }

        bool CanDoUpdate()
        {
            return CanDoExecute() && (SelectedObject != null);
        }

        void DoUpdate()
        {
            try
            {
                var values = SpectrumValues.Select(item => new SpmLKValue() { Kval = item.KValue, Lval = item.LValue }).ToList();
                if (DataModel.UpdateObjToDb(SelectedObject, ObjectName, Comment, values))
                {
                    // � ���� ������� ��������, ������ ��������� ������ � ������
                    SelectedObject.Name = ObjectName;
                    SelectedObject.Comment = Comment;
                    SelectedObject.Values = values;

                    // ��������� ��������
                    var propValues = PropertyValues.Select(propv => new SpmPropertyValue()
                    {
                        Object = SelectedObject,
                        Property = propv.GetProperty(),
                        Value = propv.GetPropertyValue()
                    }).ToList();
                    if (DataModel.UpdatePropValsToDb(propValues))
                    {
                        // �������� ������
                        RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                    }
                    else
                    {
                        DialogService.ShowMessage($"������ ���������� ������� ������� {SelectedObject.Name} � ��. ��.��� ��� ������������.");
                        return;
                    }
                    DialogService.ShowMessage($"������ {SelectedObject.Name} ������� �������� � ��");
                }
                else
                {
                    DialogService.ShowMessage($"������ ���������� ������� {SelectedObject.Name} � ��. ��.��� ��� ������������.");
                }
            }
            catch (Exception e)
            {
                DialogService.ShowMessage($"������ ���������� ������� {SelectedObject.Name}. ����� ������: {e.Message}");
            }           
        }

        void DoDelete()
        {
            var name = SelectedObject.Name;
            try
            {                
                if (DataModel.DeleteObjFromDb(SelectedObject))
                {                    
                    SelectedObject = null;                    
                    // �������� ������
                    RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();                    
                    DialogService.ShowMessage($"������ {name} ������� �������� � ��");
                }
                else
                {
                    DialogService.ShowMessage($"������ �������� ������� {name} � ��. ��.��� ��� ������������.");
                }
            }
            catch (Exception e)
            {
                DialogService.ShowMessage($"������ ���������� ������� {name}. ����� ������: {e.Message}");
            }
        }

    }
}