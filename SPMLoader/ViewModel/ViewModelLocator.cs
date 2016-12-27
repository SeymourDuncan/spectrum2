using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using Spm.Shared;

namespace SPMLoader.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            // Диалоги
            Messenger.Default.Register<DialogMessage>(this, NotificationDialogMessage);
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