using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class RegisterPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                await ((RegisterPageViewModel)BindingContext).FinishPage();

            });

        }
    }
}
