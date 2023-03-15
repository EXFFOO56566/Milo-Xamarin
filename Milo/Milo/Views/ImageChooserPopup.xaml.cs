
using System.Collections.Generic;
using Milo.Helper;
using Milo.Resources;
using Milo.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace Milo.Views
{
    public partial class ImageChooserPopup : PopupPage
    {
        private OptionMenuSelectedListener listener;
        private IList<string> OptionList { set; get; }

        public ImageChooserPopup(OptionMenuSelectedListener listener)
        {
            InitializeComponent();
            this.listener = listener;
            OptionList = new List<string>();
            OptionList.Add(AppResources.Capture_From_Camera);
            OptionList.Add(AppResources.Browse_From_Gallery);
            listView.ItemsSource = OptionList;
        }


        void listView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            string selectedOption = e.Item.ToString();
            listener.OnOptionSelected(selectedOption);
        }
    }
}
