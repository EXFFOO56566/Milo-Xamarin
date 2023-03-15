using System.Collections.Generic;
using System.Threading.Tasks;
using Milo.Resources;
using Milo.Services;
using Prism.Commands;
using Prism.Navigation;

namespace Milo.ViewModels
{
    public class ForgotPasswordPageViewModel : ViewModelBase
    {
        public DelegateCommand SubmitCommand { get; set; }
        private INavigationService _navigationService;
        private IMiloService _miloService;

        public ForgotPasswordPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            SubmitCommand = new DelegateCommand(async () => await Submit());
        }
        public async Task FinishPage()
        {
            await _navigationService.GoBackAsync();
        }
        private bool Validate()
        {
            if (string.IsNullOrEmpty(EmailAddress))
            {
                DisplayToast(AppResources.Email_Not_Entered);
                return false;
            }
            else return true;
        }
        private async Task Submit()
        {

            if (!Validate())
            {
                return;
            }
            SetBusy();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>(){
                new KeyValuePair<string, string>("email_id", EmailAddress)
            };
            var response = await _miloService.ResetPassword(request);
            SetBusy(false);

            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }

            if (response.status == 1)
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);
                await FinishPage();
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);
            }
        }

        private string _emailAddress;

        public string EmailAddress
        {
            get => _emailAddress;
            set { _emailAddress = value; NotifyPropertyChanged(); }
        }
    }
}
