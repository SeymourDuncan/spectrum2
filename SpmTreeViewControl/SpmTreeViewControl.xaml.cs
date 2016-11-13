using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using Coredata;

namespace SpmTreeViewControl
{
    /// <summary>
    /// Interaction logic for SpmTreeViewControl.xaml
    /// </summary>
    public partial class SpmTreeViewControl : UserControl, INotifyPropertyChanged
    {
        PropertyChangedEventHandler _propertyChangedHandler;
        NotifyCollectionChangedEventHandler _collectionChangedhandler;

        public SpmTreeViewControl()
        {
            InitializeComponent();
            TopLevelItems = new ObservableCollection<SpmTreeViewModelItem>();

            _propertyChangedHandler = new PropertyChangedEventHandler(item_PropertyChanged);
            _collectionChangedhandler = new NotifyCollectionChangedEventHandler(items_CollectionChanged);
            TopLevelItems.CollectionChanged += _collectionChangedhandler;
           
            LayoutRoot.DataContext = this;
        }

        private ObservableCollection<SpmTreeViewModelItem> _topLevelItems;

        public ObservableCollection<SpmTreeViewModelItem> TopLevelItems
        {
            get { return _topLevelItems; }
            set
            {
                _topLevelItems = value;
                OnPropertyChanged("TopLevelItems");
            }
        }
        #region DependendyProps
        public IList<ISpmNode> SystemItems
        {
            get { return (IList<ISpmNode>)GetValue(SystemItemsProperty); }
            set { SetValue(SystemItemsProperty, value); }
        }

        public static readonly DependencyProperty SystemItemsProperty = DependencyProperty.Register(
            "SystemItems",
            typeof (IList<ISpmNode>),
            typeof (SpmTreeViewControl),
            new UIPropertyMetadata(null,
                new PropertyChangedCallback(SystemItemsChanged)));

        public ISpmNode SelectedItem
        {
            get
            {
                return (ISpmNode)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ISpmNode), typeof(SpmTreeViewControl), new PropertyMetadata(null));


        #endregion
        public void BuildTree()
        {
            TopLevelItems.Clear();
            foreach (var sys in SystemItems)
            {
                var item = new SpmTreeViewModelItem(sys);
                TopLevelItems.Add(item);
            }
        }

        public static void SystemItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SpmTreeViewControl s = (SpmTreeViewControl) d;
            s.BuildTree();
        }

        void subscribePropertyChanged(SpmTreeViewModelItem item)
        {
            item.PropertyChanged += _propertyChangedHandler;
            item.Children.CollectionChanged += _collectionChangedhandler;
            foreach (var subitem in item.Children)
            {
                subscribePropertyChanged(subitem);
            }
        }

        void unsubscribePropertyChanged(SpmTreeViewModelItem item)
        {
            foreach (var subitem in item.Children)
            {
                unsubscribePropertyChanged(subitem);
            }
            item.Children.CollectionChanged -= _collectionChangedhandler;
            item.PropertyChanged -= _propertyChangedHandler;
        }


        void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (SpmTreeViewModelItem item in e.OldItems)
                {
                    unsubscribePropertyChanged(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (SpmTreeViewModelItem item in e.NewItems)
                {
                    subscribePropertyChanged(item);
                }
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                //OnPropertyChanged("SelectedItem");
                SelectedItem = ((SpmTreeViewModelItem) sender).Node;
            }
        }

        //
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SpmTreeViewModelItem : ViewModelBase
    {
        string _name;
        public SpmTreeViewModelItem(ISpmNode node)
        {
            Children = new ObservableCollection<SpmTreeViewModelItem>();
            Node = node;
            _name = Node.GetName();
            var childNodes = node.GetChildNodes();
            if (childNodes != null)
            {
                foreach (var nd in node.GetChildNodes())
                {
                    var item = new SpmTreeViewModelItem(nd);
                    Children.Add(item);
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public ISpmNode Node { get; set; }

        public ObservableCollection<SpmTreeViewModelItem> Children { get; private set; }

        bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
    }
}
