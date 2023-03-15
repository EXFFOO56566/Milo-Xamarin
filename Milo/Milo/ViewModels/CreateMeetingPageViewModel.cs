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
using Xamarin.Forms;

namespace Milo.ViewModels
{
    public class CreateMeetingPageViewModel : ViewModelBase
    {
        public DelegateCommand CreateMeetingCommand { get; set; }
        public DelegateCommand SelectUserCommand { get; set; }
        
        private INavigationService _navigationService;
        private IMiloService _miloService;
        public CreateMeetingPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            CreateMeetingCommand = new DelegateCommand(async () => await CreateMeeting());
            SelectUserCommand = new DelegateCommand(async () => await SelectUser());

            
            MeetingStartDate = DateTime.Now;
            MeetingStartTime = MeetingStartDate.TimeOfDay;

            MeetingEndDate = MeetingStartDate.AddHours(1);
            MeetingEndTime = MeetingEndDate.TimeOfDay;
            MeetingParticipants = "";
            MessagingCenter.Unsubscribe<string, IList<string>>("Participant", "ParticipantSelected");
            MessagingCenter.Subscribe<string, IList<string>>("Participant", "ParticipantSelected", (sender, arg) =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(MeetingParticipants) & arg.Count>0){
                        MeetingParticipants += ",";
                    }
                    MeetingParticipants += String.Join(",", arg);
                    RemoveDuplicateEmails();
                }
                catch (Exception)
                {
                }
            });
        }

        private async Task SelectUser()
        {
            await _navigationService.NavigateAsync("MultiSelectListPage", useModalNavigation: true);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            SelectedMeeting = (Meeting)parameters["SelectedMeeting"];
            if (SelectedMeeting != null)
            {
                
                PageTitle = AppResources.Update_Meeting;
                MeetingTitle = SelectedMeeting.meeting_details.title;
                MeetingAgenda = SelectedMeeting.meeting_details.agenda;
                MeetingStartDate = DateTime.Parse(SelectedMeeting.meeting_details.meeting_start_date);
                MeetingEndDate = DateTime.Parse(SelectedMeeting.meeting_details.meeting_end_date);
                MeetingStartTime = TimeSpan.Parse(SelectedMeeting.meeting_details.starting_time);
                MeetingEndTime = TimeSpan.Parse(SelectedMeeting.meeting_details.ending_time);
                List<string> emailIds = new List<string>();
                foreach(var participant in SelectedMeeting.perticipent_name)
                {
                    emailIds.Add(participant.email_id);
                }
                MeetingParticipants = String.Join(",", emailIds);
                RemoveDuplicateEmails();
            }
            else
            {
                PageTitle = AppResources.Create_Meeting;
            }
        }

        private string _pageTitle;

        public string PageTitle
        {
            get => _pageTitle;
            set { _pageTitle = value; NotifyPropertyChanged(); }
        }

        private Meeting _selectedMeeting;

        public Meeting SelectedMeeting
        {
            get => _selectedMeeting;
            set { _selectedMeeting = value; NotifyPropertyChanged(); }
        }

        
        public async Task FinishPage()
        {
            
            await _navigationService.GoBackAsync();
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(MeetingTitle) || string.IsNullOrEmpty(MeetingAgenda) || string.IsNullOrEmpty(MeetingParticipants))
            {
                DisplayToast(AppResources.All_Field_Mandatory);
                return false;
            }else if (!ValidateParticipant())
            {
                DisplayToast(AppResources.Invalid_Email);
                return false;
            }
            else return true;
        }

        private async Task CreateMeeting()
        {
            if (!Validate())
            {
                return;
            }
            SetBusy();
            RemoveDuplicateEmails();
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("title", MeetingTitle),
                new KeyValuePair<string, string>("meeting_start_date", MeetingStartDate.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("starting_time", MeetingStartTime.Hours+":"+MeetingStartTime.Minutes),
                new KeyValuePair<string, string>("meeting_end_date", MeetingEndDate.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("ending_time", MeetingEndTime.Hours+":"+MeetingEndTime.Minutes),
                new KeyValuePair<string, string>("agenda", MeetingAgenda),
                new KeyValuePair<string, string>("register_id", MeetingParticipants),
                new KeyValuePair<string, string>("token_code", token_code),
            };
            StringResponse response;
            if (SelectedMeeting == null)
            {
                response = await _miloService.CreateMeeting(request);
            }
            else
            {
                request.Add(new KeyValuePair<string, string>("update_id",SelectedMeeting.meeting_details.id));
                response = await _miloService.UpdateMeeting(request);
            }
            
            SetBusy(false);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);
                return;
            }
            if (response.status == 1)
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);
                await _navigationService.GoBackAsync();
                MessagingCenter.Send<string>("Meeting", "MeetingUpdated");
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);
            }
        }

        private void RemoveDuplicateEmails()
        {
            MeetingParticipants = String.Concat(MeetingParticipants.Where(c => !Char.IsWhiteSpace(c)));
            var list = MeetingParticipants.Split(',')?.Distinct()?.ToList();
            MeetingParticipants = String.Join(",", list);
        }
        private bool ValidateParticipant()
        {
            var list = String.Concat(MeetingParticipants.Where(c => !Char.IsWhiteSpace(c))).Split(',')?.Distinct()?.ToList();
            foreach(string email in list)
            {
                if (!MiloHelper.IsValidEmail(email))
                {
                    return false;
                }
            }
            return true;
        }
        private string _meetingTitle;

        public string MeetingTitle
        {
            get => _meetingTitle;
            set { _meetingTitle = value; NotifyPropertyChanged(); }
        }

        private string _meetingAgenda;

        public string MeetingAgenda
        {
            get => _meetingAgenda;
            set { _meetingAgenda = value; NotifyPropertyChanged(); }
        }

        private string _meetingParticipants;

        public string MeetingParticipants
        {
            get => _meetingParticipants;
            set { _meetingParticipants = value; NotifyPropertyChanged(); }
        }

        private DateTime _meetingStartDate;

        public DateTime MeetingStartDate
        {
            get => _meetingStartDate;
            set { _meetingStartDate = value; NotifyPropertyChanged(); }
        }
        private TimeSpan _meetingStartTime;

        public TimeSpan MeetingStartTime
        {
            get => _meetingStartTime;
            set { _meetingStartTime = value; NotifyPropertyChanged(); }
        }

        private DateTime _meetingEndDate;

        public DateTime MeetingEndDate
        {
            get => _meetingEndDate;
            set { _meetingEndDate = value; NotifyPropertyChanged(); }
        }
        private TimeSpan _meetingEndTime;

        public TimeSpan MeetingEndTime
        {
            get => _meetingEndTime;
            set { _meetingEndTime = value; NotifyPropertyChanged(); }
        }
    }
}
