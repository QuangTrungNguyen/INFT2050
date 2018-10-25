using UONlife.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Windows.UI.Popups;
using Windows.Storage.FileProperties;

namespace UONlife.Study
{
    public sealed partial class UploadRecordInfo : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        StorageFile file;
        StorageFolder folder;
        public UploadRecordInfo()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            string title = (string)e.Parameter;
            lblName.Text = title;
            // Load file
            folder = KnownFolders.VideosLibrary;
            file = await folder.GetFileAsync(lblName.Text);

            // Get file size
            BasicProperties properties = await file.GetBasicPropertiesAsync();
            long size = Convert.ToInt32(properties.Size);
            string sizeStr = GlobalMethod.getFileSizeStr(size);
            lblSize.Text = sizeStr;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            btnUpload.IsEnabled = false;
            btnUpload.Content = "Uploading......";
            try
            {
                // Source: https://azure.microsoft.com/en-us/documentation/articles/mobile-services-javascript-backend-windows-universal-dotnet-upload-data-blob-storage/#test
                // Part one: upload files to database
                // Create the connectionstring
                String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container. (pictures)
                CloudBlobContainer container = blobClient.GetContainerReference(GlobalVariable.loginUser.ToLower());
                await container.CreateIfNotExistsAsync();
                
                
                CloudBlockBlob blobFromSASCredential =
                                    container.GetBlockBlobReference(lblName.Text);
                await blobFromSASCredential.UploadFromFileAsync(file);
                MessageDialog msgbox = new MessageDialog("Upload success");
                await msgbox.ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                btnUpload.IsEnabled = true;
                btnUpload.Content = "Upload";
                MessageDialog msgbox = new MessageDialog(ex.Message, "Error");
                await msgbox.ShowAsync();
            }
        }
    }
}
