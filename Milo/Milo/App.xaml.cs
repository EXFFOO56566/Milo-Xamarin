using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Milo.Helper;
using Milo.Services;
using Milo.ViewModels;
using Milo.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace Milo
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=190877fb-e24d-472d-91df-12032135569a;" +
                  "ios=35090cb6-1580-46de-a8f7-d8e71e7b40eb",
                  typeof(Analytics), typeof(Crashes));
            base.OnStart();
        }
        protected override async void OnInitialized()
        {
            InitializeComponent();
            bool status = false; ;
            if (Application.Current.Properties.ContainsKey("LoginStatus"))
            {
                status = (bool)Application.Current.Properties["LoginStatus"];
            }

            if (status)
            {
                await NavigationService.NavigateAsync(nameof(MainPage) + "/" + nameof(NavigationPage) + "/" + nameof(DashboardPage));

            }
            else
            {
                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<IMiloService, MiloService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<ForgotPasswordPage, ForgotPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
            containerRegistry.RegisterForNavigation<MainPage,MainPageViewModel>();
            containerRegistry.RegisterForNavigation<PDFViewerPage, PDFViewerPageViewModel>();
            containerRegistry.RegisterForNavigation<CreateMeetingPage, CreateMeetingPageViewModel>();
            containerRegistry.RegisterForNavigation<MeetingHistoryPage, MeetingHistoryPageViewModel>();
            containerRegistry.RegisterForNavigation<UserProfilePage, UserProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<MultiSelectListPage, MultiSelectListPageViewModel>();

        }

        private bool _isMenuPresented;

        public bool IsMenuPresented
        {
            get => _isMenuPresented;
            set { _isMenuPresented = value; }
        }
    }
}
