using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Milo.ViewModels
{
    public class PDFViewerPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private string googleUrl = "https://drive.google.com/viewerng/viewer?embedded=true&url=";

        public PDFViewerPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var url = (string)parameters["TakewayDocumentLink"];
            if (Device.RuntimePlatform == Device.iOS)
            {
                TakewayDocumentLink = googleUrl + url;
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                TakewayDocumentLink = googleUrl + url;
            }
        }


        public async Task FinishPage()
        {
            await _navigationService.GoBackAsync();
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged();
            }
        }

        private string _takewayDocumentLink;

        public string TakewayDocumentLink
        {
            get { return _takewayDocumentLink; }
            set
            {
                _takewayDocumentLink = value;
                NotifyPropertyChanged();
            }
        }
    }
}
