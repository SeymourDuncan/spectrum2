/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SPMLoader"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace SPMLoader.ViewModel
{

    public class DialogMessage: MessageBase
    {
        public string Message { get; set; }
        public string Caption { get; set; }
        public bool IsError { get; set; }

        public DialogMessage(string message, string caption = "", bool isError = false)
        {
            Message = message;
            if (!isError)
            {
                Caption = caption;
            }
            else
            {
                Caption = "Error";
            }
        }
    }
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            // Диалоги
            Messenger.Default.Register<DialogMessage>(this, NotificationDialogMessage);
            // Остальное
            Messenger.Default.Register<NotificationMessage>(this, NotificationDelaultMessage);
        }

        private void NotificationDialogMessage(DialogMessage msg)
        {
            var qwe = SimpleIoc.Default.GetInstance<IDialogService>();                        
            if (msg.IsError)
            {
                qwe.ShowError(msg.Message);
            }
            else
            {
                qwe.ShowMessage(msg.Message, msg.Caption);
            }
        }

        private void NotificationDelaultMessage(NotificationMessage msg)
        {
            // TODO
        }

        public MainViewModel Main
        {
            get
            {
                var res = ServiceLocator.Current.GetInstance<MainViewModel>();
                res.DialogService = DialogService;
                return res;
            }
        }

        public IDialogService DialogService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDialogService>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}