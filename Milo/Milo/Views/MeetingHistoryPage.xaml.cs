using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class MeetingHistoryPage
    {
        public MeetingHistoryPage()
        {
            InitializeComponent();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await ((MeetingHistoryPageViewModel)BindingContext).FinishPage();

            });
        }
    }
}
