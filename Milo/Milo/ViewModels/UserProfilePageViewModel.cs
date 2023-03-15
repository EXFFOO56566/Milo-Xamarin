using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Milo.Helper;
using Milo.Model;
using Milo.Resources;
using Milo.Services;
using Milo.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Milo.ViewModels
{
    public class UserProfilePageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IMiloService _miloService;
        public DelegateCommand PickupImageCommand { get; private set; }
        public DelegateCommand UpdateProfileCommand { get; private set; }

        private ImageChooserPopup imageChooserPopup;
        private string _imageCapturedFilePath;

        public UserProfilePageViewModel(IMiloService miloService, INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _miloService = miloService;
            PickupImageCommand = new DelegateCommand(async () => await OpenPickerPopup());
            UpdateProfileCommand = new DelegateCommand(async () => await UpdateProfile());
        }

        private async Task UpdateProfile()
        {
            string token_code = await EncryptedStorageHelper.GetTokenAsync();
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("token_code", token_code),
                    new KeyValuePair<string, string>("name", FullName),
                    new KeyValuePair<string, string>("time_zone", Timezone.TimezoneValue),
                    new KeyValuePair<string, string>("address", Address),
                    new KeyValuePair<string, string>("pincode", Pin),
                    new KeyValuePair<string, string>("mobile_no", MobileNumber),
                    new KeyValuePair<string, string>("state", State),
                    new KeyValuePair<string, string>("city", City),
                };
            SetBusy();
            Stream fileStream=null;
            if (!string.IsNullOrEmpty(_imageCapturedFilePath))
            {
                fileStream = File.Open(_imageCapturedFilePath, FileMode.Open);
            }
            if(fileStream!=null && fileStream.Length> 2000000) 
            {
                await DisplayAlertAsync(AppResources.Milo, AppResources.ImageSize_Large, AppResources.OK);
                ImgCaptureImage = null;
                _imageCapturedFilePath = null;
                SetBusy(false);
                return;
            }
            var response = await _miloService.UpdateUserProfile(request, fileStream);
            SetBusy(false);
            if (response == null)
            {
                await DisplayAlertAsync(AppResources.Error, AppResources.ServerInstability, AppResources.OK);

                return;
            }
            if (response.status == 1)
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);
                await _navigationService.GoBackAsync();
                MessagingCenter.Send<string>("Profile", "ProfileUpdated");
            }
            else
            {
                await DisplayAlertAsync(AppResources.Milo, response.msg, AppResources.OK);

            }
        }
        public async Task FinishPage()
        {
            await _navigationService.GoBackAsync();
        }

        private UserProfile _userProfile;

        public UserProfile UserProfile
        {
            get => _userProfile;
            set { _userProfile = value; NotifyPropertyChanged(); }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            UserProfile = (UserProfile)parameters["UserProfile"];
            Task.Run(async () =>
            {
                SetBusy();
                Timezones = await _miloService.GetTimezones();
                Timezone = Timezones.data.FirstOrDefault(x => x.TimezoneValue == UserProfile.time_zone);
                FullName = UserProfile.name;
                Address = UserProfile.address;
                Pin = UserProfile.pincode;
                MobileNumber = UserProfile.mobile_no;
                City = UserProfile.city;
                State = UserProfile.state;

                ImgCaptureImage = ImageSource.FromUri(new Uri(UserProfile.ImageLink));

                SetBusy(false);
            });
        }

        private string _fullName;

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; NotifyPropertyChanged(); }
        }

        private string _address;

        public string Address
        {
            get => _address;
            set { _address = value; NotifyPropertyChanged(); }
        }

        private string _pin;

        public string Pin
        {
            get => _pin;
            set { _pin = value; NotifyPropertyChanged(); }
        }

        private string _mobileNumber;

        public string MobileNumber
        {
            get => _mobileNumber;
            set { _mobileNumber = value; NotifyPropertyChanged(); }
        }

        private string _city;

        public string City
        {
            get => _city;
            set { _city = value; NotifyPropertyChanged(); }
        }

        private string _state;

        public string State
        {
            get => _state;
            set { _state = value; NotifyPropertyChanged(); }
        }

        private TimezoneResponse _timezones;

        public TimezoneResponse Timezones
        {
            get => _timezones;
            set { _timezones = value; NotifyPropertyChanged(); }
        }

        private Timezone _timezone;

        public Timezone Timezone
        {
            get => _timezone;
            set { _timezone = value; NotifyPropertyChanged(); }
        }

        private ImageSource imgCaptureImage;

        public ImageSource ImgCaptureImage
        {
            set{ imgCaptureImage = value; NotifyPropertyChanged("ImgCaptureImage"); }
            get => imgCaptureImage;
            
        }

        public async Task OpenPickerPopup()
        {
            imageChooserPopup = new ImageChooserPopup(new OptionMenuSelectedListenerImpl(this));
            await PopupNavigation.Instance.PushAsync(imageChooserPopup);
        }

        private class OptionMenuSelectedListenerImpl : OptionMenuSelectedListener
        {
            private UserProfilePageViewModel viewModel;

            public OptionMenuSelectedListenerImpl(UserProfilePageViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public void OnOptionSelected(string selectedItem)
            {
                if (selectedItem == AppResources.Capture_From_Camera)
                {
                    viewModel.TakePhoto();
                }
                else if (selectedItem == AppResources.Browse_From_Gallery)
                {
                    viewModel.PickPhoto();
                }
            }

            public void OnNothingSelected()
            {
            }
        }
        public void PickPhoto()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var photosStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                    if (photosStatus != PermissionStatus.Granted)
                    {
                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;
                        photosStatus = await MiloHelper.CheckPermissions(Permission.Photos);

                        return;
                    }
                    else if (photosStatus == PermissionStatus.Granted)
                    {
                        var pickerMediaOption = new PickMediaOptions()
                        {
                            PhotoSize = PhotoSize.Full
                        };
                        IsBusy = false;
                        _imageCapturedFilePath = null;
                        ImgCaptureImage = null;
                        var file = await CrossMedia.Current.PickPhotoAsync(pickerMediaOption);

                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;

                        if (file == null)
                        {
                            IsBusy = false;
                            ImgCaptureImage = null;
                            _imageCapturedFilePath = null;
                            return;
                        }
                        _imageCapturedFilePath = file.Path;
                        ImgCaptureImage = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(_imageCapturedFilePath)));
                    }
                    else if (photosStatus != PermissionStatus.Unknown)
                    {
                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;
                    }
                }
                catch (Exception)
                {
                    await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                    imageChooserPopup = null;
                }
            });
        }

        public void TakePhoto()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                    var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                    if (cameraStatus != PermissionStatus.Granted)
                    {
                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;
                        cameraStatus = await MiloHelper.CheckPermissions(Permission.Camera);
                        return;
                    }
                    else if (storageStatus != PermissionStatus.Granted)
                    {
                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;
                        storageStatus = await MiloHelper.CheckPermissions(Permission.Storage);
                        return;
                    }
                    else if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                    {
                        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                        {
                            await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                            imageChooserPopup = null;
                            await Application.Current.MainPage.DisplayAlert(AppResources.No_Camera, AppResources.No_Camera_Msg, AppResources.OK);
                            return;
                        }
                        IsBusy = false;
                        _imageCapturedFilePath = null;
                        ImgCaptureImage = null;
                        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        {
                            Directory = Xamarin.Essentials.AppInfo.Name,
                            Name = $"{DateTime.Now}.jpg",
                            AllowCropping = true,//Both iOS and UWP have crop controls built into the the camera control when taking a photo
                            SaveToAlbum = false,//You can now save a photo or video to the camera roll/gallery
                            CompressionQuality = 92, //Set the CompressionQuality, which is a value from 0 the most compressed all the way to 100, which is no compression. A good setting from testing is around 92. This is only supported in Android & iOS
                        });
                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;
                        if (file == null)
                        {
                            IsBusy = false;
                            ImgCaptureImage = null;
                            _imageCapturedFilePath = null;
                            return;
                        }
                        _imageCapturedFilePath = file.Path;
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            byte[] byteImage;

                            using (var memoryStream = new MemoryStream())
                            {
                                file.GetStreamWithImageRotatedForExternalStorage().CopyTo(memoryStream);
                                byteImage = memoryStream.ToArray();
                                file.Dispose();
                            }
                            ImgCaptureImage = ImageSource.FromStream(() => new MemoryStream(byteImage));
                        }
                        else
                        {
                            ImgCaptureImage = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(_imageCapturedFilePath)));
                        }
                    }
                    else if (cameraStatus != PermissionStatus.Unknown || storageStatus != PermissionStatus.Unknown)
                    {
                        await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                        imageChooserPopup = null;
                    }
                }
                catch (Exception)
                {
                    await PopupNavigation.Instance.RemovePageAsync(imageChooserPopup, false);
                    imageChooserPopup = null;
                }
            });
        }

    }
}
