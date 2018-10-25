using UONlife.Common;
using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using Windows.Storage;
using Windows.UI.Popups;

namespace UONlife.Study
{
    public sealed partial class DownloadRecordInfo : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        CloudBlobContainer container;
        public DownloadRecordInfo()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            string title = (string)e.Parameter;
            lblName.Text = title;
            string downloadAddress = "https://uonlife.blob.core.windows.net/" +
                               GlobalVariable.loginUser.ToLower() + "/" +
                               title;
            lblSize.Text = downloadAddress;
        }

        private void connectToBlob()
        {
            // Create the connectionstring
            String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container. (pictures)
            container = blobClient.GetContainerReference(GlobalVariable.loginUser.ToLower());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            btnDownload.IsEnabled = false;
            btnDownload.Content = "Downloading......";
            try
            {
                // Download files and convert to file steam
                // Source: http://stackoverflow.com/questions/37248830/download-video-file-via-video-url-in-windows-phone-8-1-c-sharp
                using (HttpClient httpClient = new HttpClient())
                {
                    var data = await httpClient.GetByteArrayAsync(lblSize.Text);


                    StorageFolder storageFolder = KnownFolders.VideosLibrary;
                    var file = await storageFolder.CreateFileAsync(lblName.Text, CreationCollisionOption.GenerateUniqueName);
                    using (var targetStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await targetStream.AsStreamForWrite().WriteAsync(data, 0, data.Length);
                        await targetStream.FlushAsync();
                    }
                }
                await new MessageDialog("Download Success, please check your music library.").ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                btnDownload.IsEnabled = true;
                btnDownload.Content = "Download";
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            string itemName = lblName.Text;
            Frame.Navigate(typeof(ConfirmShare), itemName);
        }
    }
}
