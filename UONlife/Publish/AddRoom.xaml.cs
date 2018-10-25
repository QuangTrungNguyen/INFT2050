using UONlife.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.Services.Maps;
using System.Text;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.ApplicationModel.Activation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using UONlife.DataModel;

namespace UONlife
{

    public sealed partial class AddRoom : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        Geolocator geolocator = new Geolocator();
        Geoposition geoposition = null;
        double altitude = 0;
        double latitude = 0;
        CoreApplicationView view;
        StorageFile storageFile = null;


        public AddRoom()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            view = CoreApplication.GetCurrentView();

        }


        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }


        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (latitude == 0 || altitude == 0)
            {
                MessageDialog msgbox = new MessageDialog("Please provide your room's location on map");
                await msgbox.ShowAsync();
            }
            else if (storageFile == null)
            {
                MessageDialog msgbox = new MessageDialog("Please provide your room's photo");
                await msgbox.ShowAsync();
            }
            else if (tbxTitle.Text.Trim() == "")
            {
                MessageDialog msgbox = new MessageDialog("Please provide your room's title");
                await msgbox.ShowAsync();
            }
            else
            {
                try
                {
                    btnUpload.IsEnabled = false;
                    btnUpload.Content = "Uploading......";
                    string fileName = "";
                    bool imageExist = false;
                    string fileAddress = "";
                    Random r = new Random();
                    if (!storageFile.Equals(null))
                    {
                        imageExist = true;
                        // Source: https://azure.microsoft.com/en-us/documentation/articles/mobile-services-javascript-backend-windows-universal-dotnet-upload-data-blob-storage/#test
                        // Part one: upload images to database
                        // Create the connectionstring
                        String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
                        // Retrieve storage account from connection string.
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                        // Create the blob client.
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        // Retrieve a reference to a container. (pictures)
                        CloudBlobContainer container = blobClient.GetContainerReference("images");
                        await container.CreateIfNotExistsAsync();
                        string sFileName = img.Source.ToString();
                        fileName = tbxTitle.Text.Trim() + r.Next(10000000, 99999999).ToString() + ".jpg";
                        CloudBlockBlob blobFromSASCredential =
                                container.GetBlockBlobReference(fileName);
                        await blobFromSASCredential.UploadFromFileAsync(storageFile);
                    }
                    // Step two: store data into table
                    if (imageExist == true)
                    {
                        fileAddress = "https://uonlife.blob.core.windows.net/images/" + fileName;
                    }
                   
                    RentRoom rentRoom = new RentRoom
                    {
                        roomTitle = tbxTitle.Text,
                        price = tbxPrice.Text,
                        address = tbxAddress.Text,
                        type = tbxType.Text,
                        bedrooms = tbxBedrooms.Text,
                        bathrooms = tbxBathrooms.Text,
                        description = tbxDescription.Text,
                        contact = tbxContact.Text,
                        altitude = this.altitude,
                        latitude = this.latitude,
                        imageAddress = fileAddress,
                        publisher = GlobalVariable.loginUser                   
                    };
                    await App.mobileService.GetTable<RentRoom>().InsertAsync(rentRoom);
                    MessageDialog msgbox = new MessageDialog("Upload success");
                    await msgbox.ShowAsync();
                    Frame.Navigate(typeof(MainPage));
                }
                catch (Exception ex)
                {
                    MessageDialog msgbox = new MessageDialog("Error: " + ex.Message);
                    await msgbox.ShowAsync();
                    btnUpload.IsEnabled = true;
                    btnUpload.Content = "Upload";
                }
            }
        }

        private async void map_Loaded(object sender, RoutedEventArgs e)
        {
            // Authorization of map services
            map.MapServiceToken = "QRJj0BZ57yT77gTeYfN-uw";

            // Show current location
            // Source: https://msdn.microsoft.com/en-us/magazine/dn818495.aspx    
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                MessageDialog msgbox = new MessageDialog("Failed to obtain location information: " + ex.Message);
                await msgbox.ShowAsync();
            }
            map.Center = geoposition.Coordinate.Point;
            map.ZoomLevel = 15;

            // Adding a Custom Image to the Map Control
            // Source: https://msdn.microsoft.com/en-us/magazine/dn818495.aspx
            MapIcon mapIcon = new MapIcon();
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MapPin.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIcon.Location = geoposition.Coordinate.Point;
            mapIcon.Title = "You are here";
            map.MapElements.Add(mapIcon);
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (map != null)
                map.ZoomLevel = e.NewValue;
        }

        private async void LocateMe_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));
                await map.TrySetViewAsync(geoposition.Coordinate.Point, 18D);
                mySlider.Value = map.ZoomLevel;
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageDialog msgbox = new MessageDialog("Failed to obtain location information: " + ex.Message);
                await msgbox.ShowAsync();
            }
        }

        private async void map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Geopoint pointToReverseGeocode = new Geopoint(args.Location.Position);

            // Reverse geocode the specified geographic location.
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

            var resultText = new StringBuilder();

            if (result.Status == MapLocationFinderStatus.Success)
            {
                resultText.AppendLine(
                    result.Locations[0].Address.StreetNumber + ", " +
                    result.Locations[0].Address.Street + ", " +
                    result.Locations[0].Address.District + ", " +
                    result.Locations[0].Address.PostCode);
            }
            tbxAddress.Text = resultText.ToString();
            altitude = args.Location.Position.Altitude;
            latitude = args.Location.Position.Latitude;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;
        }

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            /*
             * Open camera and capture photos
             * Source: http://windowsapptutorials.com/windows-phone-8-1/media-windows-phone-8-1/using-fileopenpicker-in-windows-phone-8-1-to-choose-picture-from-picture-gallery/ 
             * */
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                storageFile = args.Files[0];
                var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                await bitmapImage.SetSourceAsync(stream);

                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                img.Source = bitmapImage;

            }
        }
    }
}
