using UONlife.Common;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;

namespace UONlife.Study
{
    public sealed partial class DownloadRecord : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public DownloadRecord()
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

            // Get file list from blob container
            String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(GlobalVariable.loginUser.ToLower());

            // Get file list from blob container
            BlobContinuationToken continuationToken = null;
            string prefix = null;
            bool useFlatBlobListing = true;
            BlobListingDetails blobListingDetails = BlobListingDetails.All;
            int maxBlobsPerRequest = 100;
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            do
            {
                var listingResult = await container.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxBlobsPerRequest, continuationToken, null, null);
                continuationToken = listingResult.ContinuationToken;
                blobs.AddRange(listingResult.Results);
            }
            while (continuationToken != null);

            // Add file list to list items
            for (int count = 0; count < blobs.Count; count ++)
            {
                string name = blobs[count].Uri.ToString().Replace(
                     "https://uonlife.blob.core.windows.net/" + 
                     GlobalVariable.loginUser.ToLower()+"/", "");
                ListItems.Items.Add(name);    
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void ListItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            string item = e.ClickedItem.ToString();
            Frame.Navigate(typeof(DownloadRecordInfo), item);
        }

    }
}
