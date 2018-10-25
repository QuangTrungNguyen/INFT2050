using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UONlife.Publish;
using UONlife.Study;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace UONlife
{
    public sealed partial class MainPage : Page
    {
       
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            tbxUsername.Text = GlobalVariable.loginUser.ToLower();

            // Connect to blob and get container size
            // Create the connectionstring
            String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container. (pictures)
            CloudBlobContainer container = blobClient.GetContainerReference(GlobalVariable.loginUser.ToLower());
            // Get container size
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void btnTextbook_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowList),"Textbook");
        }

        private void btnAccommodation_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowList), "Rent Room");
        }

        private void btnAssociation_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowList), "Society & Club");
        }


        private void btnJobs_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowList), "Job");
        }

        private void btnParty_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowList), "Party");
        }


        private void btnTextbookAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddTextbook));
        }

        private void btnAccommodationAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddRoom));
        }

        private void btnAssociationAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddSociety));
        }

        

        private void btnPartyAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddParty));
        }


        private void btnAddJobs_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddJob));
        }

        private void btnStuff_Click(object sender, RoutedEventArgs e)
        {           
            Frame.Navigate(typeof(ShowList), "Stuff");
        }

        private void btnStuffAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddStuff));
        }

        private void btnRecorder_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Recorder));
        }

        private void btnRecordCloud_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DownloadRecord));
        }

        private void btnRecordUpload_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UploadRecord));
        }

        private void btnSharedRecord_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SharedRecordList));
        }

        private void btnPortal_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Portal));
        }
    }
}
