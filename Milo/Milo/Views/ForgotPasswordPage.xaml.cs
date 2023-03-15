
using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class ForgotPasswordPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }
        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await ((ForgotPasswordPageViewModel)BindingContext).FinishPage();
            });
        }
    }
}
