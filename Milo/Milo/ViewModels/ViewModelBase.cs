using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Milo.Resources;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Milo.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, INotifyPropertyChanged
    {
        protected INavigationService NavigationService { get; private set; }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        // Todo: Not required. Can use RaisePropertyChanged in base class.
        // Todo: To be changed in all Viewmodel classes.
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {
        }

        public void SetBusy(bool busy = true, string message = "")
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (busy)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = AppResources.Processing;
                    }
                    UserDialogs.Instance.ShowLoading(message, maskType: MaskType.Gradient);
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                }
            });
            IsBusy = busy;
        }

        public async Task<bool> DisplayConfAlertAsync(string title, string message, string okText, string cancelText)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = title,
                Message = message,
                OkText = okText,
                CancelText = cancelText
            });
            return result;
        }

        public async Task DisplayAlertAsync(string title, string message, string okText)
        {
            await UserDialogs.Instance.AlertAsync(message, title, okText);
        }

        public void DisplayToast(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Acr.UserDialogs.ToastConfig.DefaultPosition = ToastPosition.Bottom;
                Acr.UserDialogs.UserDialogs.Instance.Toast(message, TimeSpan.FromSeconds(3));
            });
        }
    }
}
