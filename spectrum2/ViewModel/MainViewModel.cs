using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace spectrum2.ViewModel
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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ServerName = "localhost:3306";
        }
        string _serverName;
        private ICommand _press;

        public string ServerName {
            get
            {
                return _serverName;
            }
            set {
                RaisePropertyChanged(() => ServerName);
            }
        }
        public string UserName { get; set; } = "root";
        public string Password { get; set; } = "r2d2sat61kaz";
        public string Database { get; set; } = "spectrum2";

        public ICommand Press
        {
            get
            {
                return _press ?? (_press = new RelayCommand<object>((obj) =>
                {
                    SomeInt = true;
                }));
            }
        }

        bool _someInt = false;
        public bool SomeInt
        {
            get { return _someInt; }
            set
            {
                _someInt = value;
                RaisePropertyChanged(() => SomeInt);
            }
        }
    }
}