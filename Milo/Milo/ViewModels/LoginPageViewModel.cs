using System.Collections.Generic;
using System.Threading.Tasks;
using Milo.Helper;
using Milo.Resources;
using Milo.Services;
using Milo.Views;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Milo.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand ForgotPasswordCommand { get; set; }
        private INavigationService _navigationService;
        private IMiloService _miloService;
        public LoginPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            LoginCommand = new DelegateCommand(async () => await Login());
            RegisterCommand = new DelegateCommand(async () => await Register());
            ForgotPasswordCommand = new DelegateCommand(async () => await ForgotPassword());
        }

        private async Task ForgotPassword()
        {
            await _navigationService.NavigateAsync("ForgotPasswordPage");
        }

        private async Task Register()
        {
            await _navigationService.NavigateAsync("RegisterPage");
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(EmailAddress) || string.IsNullOrEmpty(Password))
            {
                DisplayToast(AppResources.All_Field_Mandatory);
                return false;
            }
            else return true;
        }

        private async Task Login()
        {
            if (!Validate())
            {
                return;
            }
            SetBusy();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>(){
                new KeyValuePair<string, string>("email_id", EmailAddress),
                new KeyValuePair<string, string>("password", Password)
            };
            var response = await _miloService.Login(request);
            SetBusy(false);

            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }

            if (response.status == 0)
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);
            }
            else
            {
                await EncryptedStorageHelper.SaveEmailAsync(EmailAddress);
                await EncryptedStorageHelper.SavePasswordAsync(Password);
                await EncryptedStorageHelper.SaveTokenAsync(response.token_code);
                Application.Current.Properties["LoginStatus"] = true;
                await _navigationService.NavigateAsync("/" + nameof(MainPage) + "/" + nameof(NavigationPage) + "/" + nameof(DashboardPage));
            }
        }

        private string _emailAddress;

        public string EmailAddress
        {
            get => _emailAddress;
            set { _emailAddress = value; NotifyPropertyChanged(); }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set { _password = value; NotifyPropertyChanged(); }
        }
    }
}
