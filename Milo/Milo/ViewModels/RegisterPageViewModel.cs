using System.Collections.Generic;
using System.Threading.Tasks;
using Milo.Model;
using Milo.Resources;
using Milo.Services;
using Prism.Commands;
using Prism.Navigation;

namespace Milo.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        public DelegateCommand RegisterCommand { get; set; }
        private INavigationService _navigationService;
        private IMiloService _miloService;
        public RegisterPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            RegisterCommand = new DelegateCommand(async () => await Register());
            Task.Run(async () =>
            {
                SetBusy();
                Timezones = await miloService.GetTimezones();
                SetBusy(false);
            });
        }


        private TimezoneResponse _timezones;

        public TimezoneResponse Timezones
        {
            get => _timezones;
            set { _timezones = value; NotifyPropertyChanged(); }
        }

        public async Task FinishPage()
        {
            await _navigationService.GoBackAsync();
        }
        private bool Validate()
        {
            if (string.IsNullOrEmpty(FullName) ||
                string.IsNullOrEmpty(EmailAddress) ||
                string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(ConfPassword) ||
                Timezone == null)
            {
                DisplayToast(AppResources.All_Field_Mandatory);
                return false;
            }
            else if (Password != ConfPassword)
            {
                DisplayToast(AppResources.Password_Cnf_Password_NotMatching);
                return false;
            }
            else return true;


        }
        private async Task Register()
        {
            if (!Validate())
            {
                return;
            }
            SetBusy();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("name", FullName),
                new KeyValuePair<string, string>("email_id", EmailAddress),
                new KeyValuePair<string, string>("password", Password),
                new KeyValuePair<string, string>("confirm_password", ConfPassword),
                new KeyValuePair<string, string>("time_zone", Timezone.TimezoneValue),
            };
            var responseRegistration = await _miloService.Register(request);

            
            SetBusy(false);
            if (responseRegistration == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);

                return;
            }

            if (responseRegistration.status == 1)
            {
                await DisplayAlertAsync(AppResources.Milo, responseRegistration.msg, AppResources.OK);
                SetBusy();
                List<KeyValuePair<string, string>> requestVerifyLink = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("email_id", EmailAddress),
                };
                var responseVerifyLink = await _miloService.SendVerifyLink(requestVerifyLink);
                SetBusy(false);
                await DisplayAlertAsync(AppResources.Milo, responseVerifyLink.msg, AppResources.OK);
                await _navigationService.GoBackAsync();
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, responseRegistration.msg, AppResources.OK);

            }
        }

        private string _emailAddress;

        public string EmailAddress
        {
            get => _emailAddress;
            set { _emailAddress = value; NotifyPropertyChanged(); }
        }

        private string _fullName;

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; NotifyPropertyChanged(); }
        }

        private Timezone _timezone;

        public Timezone Timezone
        {
            get => _timezone;
            set { _timezone = value; NotifyPropertyChanged(); }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set { _password = value; NotifyPropertyChanged(); }
        }

        private string _confPassword;

        public string ConfPassword
        {
            get => _confPassword;
            set { _confPassword = value; NotifyPropertyChanged(); }
        }

    }
}
