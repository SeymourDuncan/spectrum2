using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coredata;
using GalaSoft.MvvmLight;

namespace SPMLoader
{
    public class SpmTreeViewModelItem : ViewModelBase
    {
        string _name;

        public SpmTreeViewModelItem(ISpmNode node)
        {
            Children = new ObservableCollection<SpmTreeViewModelItem>();
            Name = node.GetName();
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

    public class SpmTreeViewModel : ViewModelBase
    {
        PropertyChangedEventHandler _propertyChangedHandler;
        NotifyCollectionChangedEventHandler _collectionChangedhandler;

        public SpmTreeViewModel()
        {
            TopLevelItems = new ObservableCollection<SpmTreeViewModelItem>();
            _propertyChangedHandler = new PropertyChangedEventHandler(item_PropertyChanged);
            _collectionChangedhandler = new NotifyCollectionChangedEventHandler(items_CollectionChanged);
            TopLevelItems.CollectionChanged += _collectionChangedhandler;
        }

        public ObservableCollection<SpmTreeViewModelItem> TopLevelItems { get; private set; }

        public SpmTreeViewModelItem SelectedItem
        {
            get
            {
                // TODO найти детей, найти первый который IsSelected
                return TopLevelItems[0];
                //.Traverse(item => item.Children)
                //.FirstOrDefault(m => m.IsSelected);
            }
        }

        public void BuildTree(IList<SpmSystem> model)
        {
            TopLevelItems.Clear();
            foreach (var sys in model)
            {
                var item = new SpmTreeViewModelItem(sys);
                TopLevelItems.Add(item);
            }
        }

        void SubscribePropertyChanged(SpmTreeViewModelItem item)
        {
            item.PropertyChanged += _propertyChangedHandler;
            item.Children.CollectionChanged += _collectionChangedhandler;
            foreach (var subitem in item.Children)
            {
                SubscribePropertyChanged(subitem);
            }
        }

        void UnsubscribePropertyChanged(SpmTreeViewModelItem item)
        {
            foreach (var subitem in item.Children)
            {
                UnsubscribePropertyChanged(subitem);
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
                    UnsubscribePropertyChanged(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (SpmTreeViewModelItem item in e.NewItems)
                {
                    SubscribePropertyChanged(item);
                }
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                // хм. А зачем?
                RaisePropertyChanged(() => SelectedItem);
            }
        }
    }

}
