using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using spectrum2.View;
using Spm.Shared;
using System.Windows;
using spectrum2.ViewModel;

namespace Spectrum2.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ConnSettingsViewModel>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            // Диалоги
            Messenger.Default.Register<DialogMessage>(this, NotificationDialogMessage);

            Messenger.Default.Register<ShowDialogMessage>(this, CreateDialogWindow);
        }

        public MainViewModel Main
        {
            get
            {                
                var res = ServiceLocator.Current.GetInstance<MainViewModel>();
                res.DiagService = DialogService;
                return res;
            }
        }

        public ConnSettingsViewModel ConnSettings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConnSettingsViewModel>();                                
            }
        }

        private void NotificationDialogMessage(DialogMessage msg)
        {            
            if (msg.IsError)
            {
                DialogService.ShowError(msg.Message);
            }
            else
            {
                DialogService.ShowMessage(msg.Message, msg.Caption);
            }
        }

        public IDialogService DialogService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDialogService>();
            }
        }

        public void CreateDialogWindow(ShowDialogMessage msg)
        {
            switch (msg.Type)
            {
                case DialogWindowTypes.DwConnectionSettings:
                    var view = new ConnSettingsView()
                    {
                        Owner = Application.Current.MainWindow,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    ConnSettings.Clear();
                    ConnSettings.SetConnData(Main.ConnData);
                    ConnSettings.ChangeConnData += Main.OnConnDataChanged;
                    ConnSettings.CloseDialogAction = view.Close;
                    view.ShowDialog();
                    break;
            }
        }

        private void ConnSettings_ChangeConnData(object sender, spectrum2.Model.ConnectionData e)
        {
            throw new System.NotImplementedException();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}