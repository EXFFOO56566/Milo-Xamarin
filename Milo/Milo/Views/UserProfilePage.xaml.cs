
using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class UserProfilePage
    {
        public UserProfilePage()
        {
            InitializeComponent();
        }
        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await ((UserProfilePageViewModel)BindingContext).FinishPage();


            });
        }
    }
}
