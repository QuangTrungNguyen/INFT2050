using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using UONlife.Common;
using UONlife.DataModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace UONlife
{
    public sealed partial class AddTextbook : Page
    {
        // Claim variables 
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        CoreApplicationView view;
        StorageFile storageFile = null;
        public AddTextbook()
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

        private async void btnUpload_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {   if (tbxName.Text.Trim() == "")
            {
                MessageDialog msgbox = new MessageDialog("You must provide a valid book name");
                await msgbox.ShowAsync();
            }
            else if (storageFile == null)
            {
                MessageDialog msgbox = new MessageDialog("You must provide a valid book picture");
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
                        fileName = tbxName.Text.Trim() + r.Next(10000000, 99999999).ToString() + ".jpg";
                        CloudBlockBlob blobFromSASCredential =
                                container.GetBlockBlobReference(fileName);
                        await blobFromSASCredential.UploadFromFileAsync(storageFile);
                    }
                    // Step two: store data into table
                    if (imageExist == true)
                    {
                        fileAddress = "https://uonlife.blob.core.windows.net/images/" + fileName;
                    }
                    Textbook textbook = new Textbook
                    {
                        bookName = tbxName.Text,
                        courseID = tbxCourseID.Text,
                        depreciation = tbxDepreciation.Text,
                        price = tbxPrice.Text,
                        description = tbxDescription.Text,
                        contact = tbxContact.Text,
                        imageAddress = fileAddress,
                        publisher = GlobalVariable.loginUser
                    };
                    await App.mobileService.GetTable<Textbook>().InsertAsync(textbook);

                    MessageDialog msgbox = new MessageDialog("Upload success");
                    await msgbox.ShowAsync();

                    Frame.Navigate(typeof(MainPage));
                }
                catch (Exception ex)
                {
                    MessageDialog msgbox = new MessageDialog("Error: " + ex.Message);
                    await msgbox.ShowAsync();
                    btnUpload.IsEnabled = true;
                    btnUpload.Content = "Upload File To Cloud";
                }
            }                  
        }

        private void btnSelect_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
    }
}
