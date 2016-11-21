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

        public MainViewModel()
        {            
            DataModel = new SpmStorage();            
            _spectrumValues = new ObservableCollection<SpmDataGridItem>();
            _propertyValues = new ObservableCollection<DummyPropValue>();
            ExecuteCommand = new AutoRelayCommand(DoExecute, CanDoExecute);

            ExecuteCommand.DependsOn(() => SpectrumValues);
            ExecuteCommand.DependsOn(() => ObjectName);
            ExecuteCommand.DependsOn(() => SelectedClass);
            ExecuteCommand.DependsOn(() => SelectedSystem);
        }

        SpmStorage DataModel { get; set; }        
        public IDialogService DialogService { get; set; }

        public string ServerName { get; set; } = "localhost";
        public string UserName { get; set; } = "root";
        public string Password { get; set; } = "r2d2sat61kaz";
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
                DeterminParentNames();
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
                // Так отсылается сообщение мессаджеру
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

        /// <summary>
        /// Обновляем значения в таблице Свойства
        /// </summary>
        void UpdatePropertyValues()
        {
            PropertyValues.Clear();
            foreach (var prop in SelectedSystem.Properties.Properties)
            {
                var dummyProp = prop.Type == SpmTypeEnum.stDictType ? new DummyDictPropValue(prop) : new DummyPropValue(prop);
                PropertyValues.Add(dummyProp);
            }            
        }

        /// <summary>
        /// Находим Систему и Класс выбранного элемента
        /// </summary>
        void DeterminParentNames()
        {
            SelectedClass = null;
            SelectedSystem = null;
            switch (SelectedNode.GetNodeType())
            {
                case SpmNodeType.SntSystem:
                {
                    SelectedSystem = (SpmSystem)SelectedNode;
                    break;
                }
                case SpmNodeType.SntClass:
                {
                    var cl = (SpmClass) SelectedNode;
                    SelectedClass = cl;
                    SelectedSystem = cl?.ParentSystem;
                    break;
                }
                case SpmNodeType.SntObject:
                {
                    var obj = (SpmObject) SelectedNode;                    
                    SelectedSystem = obj.System;
                    SelectedClass = obj.Class;
                    break;
                }
            }
        }

        public ICommand ConnectCommand => _connectCommand?? (_connectCommand = new RelayCommand<object>((obj) =>
        {
            if (DataModel.Connect(ServerName, UserName, Password, Database))
            {
                Status = "Подключение выполнено. Обновляется дерево спектров.";
                try
                {
                    RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                }
                catch (Exception e)
                {
                    Status = $"Обновление дерева спектров не удалось. Причина: {e.Message}";
                    return;
                }
                Status = "Дерево спектров обновлено.";
            }
            else
            {
                Status = "Не удалось выполнить подключение к БД.";
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
                        // пробуем обработать                       
                        using (var parser = new FileParser())
                        {
                            if (parser.ParseFile(FileName))
                            {
                                parser.RowDoubleValues.ForEach(arr =>
                                {
                                    SpectrumValues.Add(new SpmDataGridItem() { LValue = arr[0], KValue = arr[1] });
                                });
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
                }));
            }
        }


        public AutoRelayCommand ExecuteCommand { get; set; }

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
            var spmObj = new SpmObject(0, ObjectName, SelectedSystem, Comment);
            spmObj.Class = SelectedClass;
            spmObj.Values = SpectrumValues.ToDictionary(item => item.LValue, item => item.KValue);

            // сохраняем объект
            if (DataModel.SaveObjToDb(spmObj))
            {
                // освежаем дерево
                RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                DialogService.ShowMessage($"Объект {spmObj.Name} успешно загружен в БД");
            }
            else
            {
                DialogService.ShowMessage($"Ошибка загрузки объекта {spmObj.Name} в БД. См.лог для подробностей.");
                return;
            }
           
            // сохраняет свойства     
            var propValues = PropertyValues.Select(propv => new SpmPropertyValue()
            {
                Object = spmObj, Property = propv.GetProperty(), Value = propv.GetPropertyValue()
            }).ToList();

            if (DataModel.SavePropValsToDb(propValues))
            {
                // освежаем дерево
                RootNodes = DataModel.Model.Cast<ISpmNode>().ToList();
                DialogService.ShowMessage($"Объект {spmObj.Name} успешно загружен в БД");
            }
            else
            {
                DialogService.ShowMessage($"Ошибка загрузки свойств объекта {spmObj.Name} в БД. См.лог для подробностей.");                
            }

        }
    }
}