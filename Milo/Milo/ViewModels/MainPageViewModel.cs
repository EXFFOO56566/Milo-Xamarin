using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Navigation;
using Milo.Model;
using Milo.Views;
using Xamarin.Forms;
using Milo.Helper;
using Milo.Resources;

namespace Milo.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        public ObservableCollection<MyMenuItem> MenuItems { get; set; }

        private string _previousLoadedPage = nameof(DashboardPage);

        private MyMenuItem selectedMenuItem;
        public MyMenuItem SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }


        private bool _isMenuPresented;

        public bool IsMenuPresented
        {
            get => _isMenuPresented;
            set {
                _isMenuPresented = value;
                ((App)App.Current).IsMenuPresented = IsMenuPresented;
                NotifyPropertyChanged();
            }
        }

        public DelegateCommand NavigateCommand { get; private set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            MenuItems = new ObservableCollection<MyMenuItem>();

            MenuItems.Add(new MyMenuItem()
            {
                Icon = "home.png",
                PageName = nameof(DashboardPage),
                Title = AppResources.Home
            });
            MenuItems.Add(new MyMenuItem()
            {
                Icon = "meeting.png",
                PageName = nameof(CreateMeetingPage),
                Title = AppResources.Create_Meeting
            });
            MenuItems.Add(new MyMenuItem()
            {
                Icon = "history.png",
                PageName = nameof(MeetingHistoryPage),
                Title = AppResources.Meeting_History
            });
            MenuItems.Add(new MyMenuItem()
            {
                Icon = "logout.png",
                PageName = nameof(LoginPage),
                Title = AppResources.Logout
            });

            NavigateCommand = new DelegateCommand(Navigate);

            MessagingCenter.Unsubscribe<string, bool>("Milo", "OpenNavigation");
            MessagingCenter.Subscribe<string, bool>("Milo", "OpenNavigation", (sender, arg) =>
            {
                IsMenuPresented = arg;
            });
        }

        async void Navigate()
        {
            if(SelectedMenuItem.Title== AppResources.Logout)
            {
                if (!await DisplayConfAlertAsync(AppResources.Logout, AppResources.LogOutAreYouSure, AppResources.Yes, AppResources.No))
                    return;

                EncryptedStorageHelper.ClearCache();
                await _navigationService.NavigateAsync("/" + nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            }
            else if (SelectedMenuItem.Title == AppResources.Create_Meeting || SelectedMenuItem.Title == AppResources.Meeting_History)
            {
                await _navigationService.NavigateAsync(nameof(MainPage) + "/" + nameof(NavigationPage) + "/" + _previousLoadedPage + "/" + SelectedMenuItem.PageName);
            }
            else
            {
                await _navigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            }
            _previousLoadedPage = SelectedMenuItem.PageName;
        }
    }
}
