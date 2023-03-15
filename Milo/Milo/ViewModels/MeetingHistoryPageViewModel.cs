using System.Collections.Generic;
using System.Threading.Tasks;
using Milo.Helper;
using Milo.Model;
using Milo.Resources;
using Milo.Services;
using Prism.Navigation;

namespace Milo.ViewModels
{
    public class MeetingHistoryPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IMiloService _miloService;
        public MeetingHistoryPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            

            Task.Run(async() =>
            {
                string token_code = await EncryptedStorageHelper.GetTokenAsync();
                List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("token_code", token_code),
            };
                SetBusy();
                var response = await _miloService.GetMeetingHistory(request);
                SetBusy(false);
                if (response == null)
                {
                    await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                    return;
                }
                MeetingHistory = response.data;
            });
            
        }

        public async Task FinishPage()
        {
            await _navigationService.GoBackAsync();
        }
        
        private List<MeetingHistory> _meetingHistory;

        public  List<MeetingHistory> MeetingHistory
        {
            get => _meetingHistory;
            set { _meetingHistory = value; NotifyPropertyChanged(); }
        }
    }
}
