using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class CreateMeetingPage
    {
        public CreateMeetingPage()
        {
            InitializeComponent();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async() =>
            {
                await ((CreateMeetingPageViewModel)BindingContext).FinishPage();

            });
        }
    }
}
