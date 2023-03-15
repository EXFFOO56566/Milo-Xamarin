using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milo.Helper;
using Milo.Model;
using Milo.Resources;
using Milo.Services;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Milo.ViewModels
{
    public class DashboardPageViewModel : ViewModelBase
    {
        public DelegateCommand OpenCloseNavCommand { get; set; }

        public DelegateCommand<Meeting> AcceptedBookingViewCommand { get; set; }
        public DelegateCommand<Meeting> AcceptedBookingAgendaCommand { get; set; }
        public DelegateCommand<Meeting> AcceptedBookingTakeawayCommand { get; set; }
        public DelegateCommand<Meeting> AcceptedBookingEditCommand { get; set; }
        public DelegateCommand<Meeting> AcceptedBookingShareCommand { get; set; }
        public DelegateCommand<Meeting> AcceptedBookingJoinCommand { get; set; }
        public DelegateCommand<Meeting> AcceptedBookingCancelCommand { get; set; }

        public DelegateCommand<Meeting> InvitedBookingViewCommand { get; set; }
        public DelegateCommand<Meeting> InvitedBookingAgendaCommand { get; set; }
        public DelegateCommand<Meeting> InvitedBookingTakeawayCommand { get; set; }
        public DelegateCommand<Meeting> InvitedBookingEditCommand { get; set; }
        public DelegateCommand<Meeting> InvitedBookingShareCommand { get; set; }
        public DelegateCommand<Meeting> InvitedBookingAcceptCommand { get; set; }
        public DelegateCommand<Meeting> InvitedBookingRejectCommand { get; set; }

        public DelegateCommand ShowProfileCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }

        

        private INavigationService _navigationService;
        private IMiloService _miloService;
        public DashboardPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            OpenCloseNavCommand = new DelegateCommand( () =>  OpenCloseNav());

            AcceptedBookingViewCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingView(x));
            AcceptedBookingAgendaCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingAgenda(x));
            AcceptedBookingTakeawayCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingTakeaway(x));
            AcceptedBookingEditCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingEdit(x));
            AcceptedBookingShareCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingShare(x));
            AcceptedBookingJoinCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingJoin(x));
            AcceptedBookingCancelCommand = new DelegateCommand<Meeting>(async (x) => await AcceptedBookingCancel(x));

            InvitedBookingViewCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingView(x));
            InvitedBookingAgendaCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingAgenda(x));
            InvitedBookingTakeawayCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingTakeaway(x));
            InvitedBookingEditCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingEdit(x));
            InvitedBookingShareCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingShare(x));
            InvitedBookingAcceptCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingAccept(x));
            InvitedBookingRejectCommand = new DelegateCommand<Meeting>(async (x) => await InvitedBookingReject(x));

            ShowProfileCommand = new DelegateCommand(async () => await ShowProfile());
            RefreshCommand = new DelegateCommand(async () => await Refresh());
            
            Task.Run(async() =>
            {
                await Refresh();
            });
            MessagingCenter.Unsubscribe<string>("Profile", "ProfileUpdated");
            MessagingCenter.Subscribe<string>("Profile", "ProfileUpdated", async(sender) =>
            {
                await LoadUserProfile();
            });
            MessagingCenter.Unsubscribe<string>("Meeting", "MeetingUpdated");
            MessagingCenter.Subscribe<string>("Meeting", "MeetingUpdated", async (sender) =>
            {
                await LoadData();
            });
        }

        private async Task Refresh()
        {
            await LoadData();
            await LoadUserProfile();
        }

        private async Task ShowProfile()
        {
            var navParameters = new NavigationParameters
            {
                { "UserProfile", UserProfile },
            };
            await _navigationService.NavigateAsync("UserProfilePage", navParameters);
        }

        private async Task LoadUserProfile()
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                };
            var result = await _miloService.ViewUserProfile(request);
            if (result == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            UserProfile = result.data;
        }
        private async Task LoadData()
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                };
            SetBusy();
            var response = await _miloService.GetMeetings(request);
            
            SetBusy(false);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            List<Meeting> accepted, invited;
            accepted = new List<Meeting>();
            invited = new List<Meeting>();
            foreach (Meeting meeting in response.data)
            {
                if (meeting.my_status.status == "1")
                {
                    accepted.Add(meeting);
                }
                else
                {
                    invited.Add(meeting);
                }
            }
            AcceptedMeetings = accepted.OrderBy(s => MergeTime(s.meeting_details.meeting_start_date, s.meeting_details.starting_time)).ToList();
            InvitedMeetings = invited.OrderBy(s => MergeTime(s.meeting_details.meeting_start_date, s.meeting_details.starting_time)).ToList();
        }

        public static DateTime MergeTime(string strDate, string strTime)
        {
            DateTime date = System.Convert.ToDateTime(strDate);

            TimeSpan time = TimeSpan.Parse(strTime);

            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        private List<Meeting> _invitedMeetings;

        public  List<Meeting> InvitedMeetings
        {
            get => _invitedMeetings;
            set { _invitedMeetings = value; NotifyPropertyChanged(); }
        }

        private List<Meeting> _acceptedMeetings;

        public List<Meeting> AcceptedMeetings
        {
            get => _acceptedMeetings;
            set { _acceptedMeetings = value; NotifyPropertyChanged(); }
        }

        private UserProfile _userProfile;

        public UserProfile UserProfile
        {
            get => _userProfile;
            set { _userProfile = value; NotifyPropertyChanged(); }
        }

        private void OpenCloseNav()
        {
            ((App)App.Current).IsMenuPresented = !((App)App.Current).IsMenuPresented;
            MessagingCenter.Send<string, bool>("Milo", "OpenNavigation", ((App)App.Current).IsMenuPresented);
        }

        private async Task InvitedBookingReject(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("id", x.meeting_details.id)
                };
            SetBusy();
            var response = await _miloService.RejectMeetingInvitation(request);

            SetBusy(false);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }

            DisplayToast(response.msg);
            if (response.status == 1)
            {
                await LoadData();
            }
        }

        private async Task InvitedBookingAccept(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("id", x.meeting_details.id)
                };
            SetBusy();
            var response = await _miloService.AcceptMeetingInvitation(request);
            SetBusy(false);
            if (response == null)
            {

                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);

                return;
            }
            DisplayToast(response.msg);
            if (response.status == 1)
            {
                await LoadData();
            }
        }

        private async Task InvitedBookingShare(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("meeting_id", x.meeting_details.id)
                };
            SetBusy();
            var response = await _miloService.JoinMeeting(request);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            SetBusy(false);
            if (response.status == 1)
            {
                try
                {

                    await MiloHelper.Share(x, response.data);
                }
                catch (Exception)
                {
                    // An unexpected error occured. No browser may be installed on the device.
                }
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);

            }
        }

        private async Task InvitedBookingEdit(Meeting x)
        {
            var navParameters = new NavigationParameters
            {
                { "SelectedMeeting", x },
            };
            await _navigationService.NavigateAsync("CreateMeetingPage", navParameters);
        }

        private async Task InvitedBookingTakeaway(Meeting x)
        {
            if (string.IsNullOrEmpty(x.meeting_details.takeaways))
            {
                await DisplayAlertAsync(AppResources.Milo, AppResources.Takeway_not_available, AppResources.OK);

                return;
            }
            var navParameters = new NavigationParameters
            {
                { "TakewayDocumentLink", x.takeway },
            };
            await _navigationService.NavigateAsync("PDFViewerPage", navParameters);
        }

        private async Task InvitedBookingAgenda(Meeting x)
        {
            await DisplayAlertAsync(AppResources.Meeting_Agenda, x.meeting_details.agenda, "OK");
        }

        private async Task InvitedBookingView(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            var navParameters = new NavigationParameters
            {
                { "TakewayDocumentLink", AppSettings.BaseUrl +"api/get-takeway/"+token_code+"/"+x.meeting_details.id },
            };
            await _navigationService.NavigateAsync("PDFViewerPage", navParameters);

        }

        private async Task AcceptedBookingCancel(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("id", x.meeting_details.id)
                };
            SetBusy();
            var response = await _miloService.CancelMeeting(request);
            SetBusy(false);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            DisplayToast(response.msg);
            if (response.status == 1)
            {
                await LoadData();
            }
        }

        private async Task AcceptedBookingJoin(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("meeting_id", x.meeting_details.id)
                };
            SetBusy();
            var response = await _miloService.JoinMeeting(request);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            SetBusy(false);
            if (response.status == 1)
            {
                try
                {
                    var platform = DeviceInfo.Platform;
                    var bowserLaunchMode = BrowserLaunchMode.External;
                    if(platform == DevicePlatform.iOS)
                    {
                        var osMajorVersion = int.Parse(DeviceInfo.VersionString.Split('.')[0]);
                        if (osMajorVersion >= 13)
                        {
                            bowserLaunchMode = BrowserLaunchMode.SystemPreferred;
                        }
                    }
                    await Browser.OpenAsync(response.data, bowserLaunchMode);
                }
                catch (Exception)
                {
                    // An unexpected error occured. No browser may be installed on the device.
                }
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);

            }

        }

        private async Task AcceptedBookingShare(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("meeting_id", x.meeting_details.id)
                };
            SetBusy();
            var response = await _miloService.JoinMeeting(request);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            SetBusy(false);
            if (response.status == 1)
            {
                try
                {

                    await MiloHelper.Share(x,response.data);
                }
                catch (Exception)
                {
                    // An unexpected error occured. No browser may be installed on the device.
                }
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);

            }
            
        }

        private async Task AcceptedBookingEdit(Meeting x)
        {
            var navParameters = new NavigationParameters
            {
                { "SelectedMeeting", x },
            };
            await _navigationService.NavigateAsync("CreateMeetingPage", navParameters);
        }

        private async Task AcceptedBookingTakeaway(Meeting x)
        {
            if (string.IsNullOrEmpty(x.meeting_details.takeaways))
            {
                await DisplayAlertAsync(AppResources.Milo, AppResources.Takeway_not_available, AppResources.OK);
                return;
            }
            var navParameters = new NavigationParameters
            {
                { "TakewayDocumentLink", x.meeting_details.takeaways },
            };
            await _navigationService.NavigateAsync("PDFViewerPage", navParameters);
        }

        private async Task AcceptedBookingAgenda(Meeting x)
        {
            await DisplayAlertAsync(AppResources.Meeting_Agenda, x.meeting_details.agenda, "OK");
        }

        private async Task AcceptedBookingView(Meeting x)
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            var navParameters = new NavigationParameters
            {
                { "TakewayDocumentLink", AppSettings.BaseUrl +"api/get-takeway/"+token_code+"/"+x.meeting_details.id },
            };
            await _navigationService.NavigateAsync("PDFViewerPage", navParameters);
        }
    }
}
