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
    public class MultiSelectListPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IMiloService _miloService;

        private System.Timers.Timer timer;

        private double totalWaitingTime = 500;//Progress bar time and time to submit the api call

        public MultiSelectListPageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            FinishText = AppResources.Finish;
            timer = new System.Timers.Timer { Interval = totalWaitingTime }; // in milliseconds
            timer.Elapsed += (s, e) =>
            {
                Task.Run(async () => await SearchUserAsync());
            };

        }
        private IList<SelectableData> _dataList;

        public IList<SelectableData> DataList
        {
            get => _dataList;
            set { _dataList = value; NotifyPropertyChanged(); }
        }

        private string _finishText;

        public string FinishText
        {
            get => _finishText;
            set { _finishText = value; NotifyPropertyChanged(); }
        }

        private string _keyWord;

        public string KeyWord
        {
            get => _keyWord;
            set { _keyWord = value; NotifyPropertyChanged(); }
        }

        public List<string> GetSelectedData()
        {
            var list = new List<string>();
            try
            {
                foreach (var data in DataList)
                    if (data.Selected)
                        list.Add(data.email_id);
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public DelegateCommand FinishCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    try
                    {
                        List<string> selectedData = new List<string>();
                        if (FinishText == AppResources.Finish)
                        {
                            selectedData = new List<string>();
                            selectedData = GetSelectedData();
                        }
                        else if (FinishText == AppResources.Add_as_guest)
                        {
                            selectedData = new List<string>();
                            selectedData.Add(KeyWord);
                        }
                        MessagingCenter.Send<string, IList<string>>("Participant", "ParticipantSelected", selectedData);
                        await _navigationService.GoBackAsync(useModalNavigation: true);
                    }
                    catch (Exception ex)
                    {
                    }
                });
            }
        }

        public async Task SearchUserAsync()
        {
            StopTimer();
            if (string.IsNullOrEmpty(KeyWord))
            {
                DataList = null;
                return;
            }
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("search_key", KeyWord)
                };
            var response = await _miloService.SearchUser(request);
            if (response != null && response.data != null && response.data.Count > 0)
            {
                DataList = response.data;
            }
            else
            {
                DataList = null;
            }

            if (DataList != null && DataList.Count > 0)
            {
                FinishText = AppResources.Finish;
            }
            else
            {
                if (MiloHelper.IsValidEmail(KeyWord))
                {
                    FinishText = AppResources.Add_as_guest;
                }
                else
                {
                    FinishText = AppResources.Finish;
                }
            }
        }
    }
}