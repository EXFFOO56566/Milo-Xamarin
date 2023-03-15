using System;
using System.Threading.Tasks;
using Milo.ViewModels;
using Xamarin.Forms;

namespace Milo.Views
{
    public partial class MultiSelectListPage
    {
        private MultiSelectListPageViewModel _viewModel;
        public MultiSelectListPage()
        {
            InitializeComponent();
            _viewModel = (MultiSelectListPageViewModel)BindingContext;
        }
        void fullNameEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _viewModel.KeyWord = e.NewTextValue;
            _viewModel.StopTimer();
            _viewModel.StartTimer();
        }
    }
}