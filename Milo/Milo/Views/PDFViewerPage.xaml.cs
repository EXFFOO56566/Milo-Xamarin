
using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class PDFViewerPage
    {
        private PDFViewerPageViewModel _viewModel;

        public PDFViewerPage()
        {
            InitializeComponent();
            _viewModel = (PDFViewerPageViewModel)BindingContext;
        }
        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.FinishPage();
            });
        }

        private void WebView_Navigating(System.Object sender, Xamarin.Forms.WebNavigatingEventArgs e)
        {
            _viewModel.IsLoading = true;
        }

        private void WebView_Navigated(System.Object sender, Xamarin.Forms.WebNavigatedEventArgs e)
        {
            _viewModel.IsLoading = false;
        }

    }
}
