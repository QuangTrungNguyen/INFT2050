using UONlife.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;
using Windows.UI.Popups;
using UONlife.Content;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using UONlife.Study;
using UONlife.ReviewSystem;

namespace UONlife
{
    public sealed partial class Portal : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        
        // Collection variables           
        private MobileServiceCollection<Textbook, Textbook> TextbookItems;
        private IMobileServiceTable<Textbook> TextbookTable = App.mobileService.GetTable<Textbook>();

        private MobileServiceCollection<Job, Job> JobItems;
        private IMobileServiceTable<Job> JobTable = App.mobileService.GetTable<Job>();

        private MobileServiceCollection<RentRoom, RentRoom> RentRoomItems;
        private IMobileServiceTable<RentRoom> RentRoomTable = App.mobileService.GetTable<RentRoom>();

        private MobileServiceCollection<Society, Society> SocietyItems;
        private IMobileServiceTable<Society> SocietyTable = App.mobileService.GetTable<Society>();

        private MobileServiceCollection<Party, Party> PartyItems;
        private IMobileServiceTable<Party> PartyTable = App.mobileService.GetTable<Party>();

        private MobileServiceCollection<Stuff, Stuff> StuffItems;
        private IMobileServiceTable<Stuff> StuffTable = App.mobileService.GetTable<Stuff>();

        private MobileServiceCollection<SharedAudio, SharedAudio> SharedAudioItems;
        private IMobileServiceTable<SharedAudio> SharedAudioTable = App.mobileService.GetTable<SharedAudio>();

        private MobileServiceCollection<Comments, Comments> PostedCommentsItems;
        private IMobileServiceTable<Comments> PostedCommentsTable = App.mobileService.GetTable<Comments>();

        private MobileServiceCollection<Comments, Comments>ReceivedCommentsItems;
        private IMobileServiceTable<Comments> ReceivedCommentsTable = App.mobileService.GetTable<Comments>();
        public Portal()
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

            MobileServiceInvalidOperationException exception = null;
            try
            {
                TextbookItems = await TextbookTable.Where(Textbook => Textbook.publisher == GlobalVariable.loginUser).ToCollectionAsync();
                JobItems = await JobTable.Where(Job => Job.publisher == GlobalVariable.loginUser).ToCollectionAsync();
                RentRoomItems = await RentRoomTable.Where(RentRoom => RentRoom.publisher == GlobalVariable.loginUser).ToCollectionAsync();
                SocietyItems = await SocietyTable.Where(Society => Society.publisher == GlobalVariable.loginUser).ToCollectionAsync();
                PartyItems = await PartyTable.Where(Party => Party.publisher == GlobalVariable.loginUser).ToCollectionAsync();
                StuffItems = await StuffTable.Where(Stuff => Stuff.publisher == GlobalVariable.loginUser).ToCollectionAsync();
                SharedAudioItems = await SharedAudioTable.Where(SharedAudio => SharedAudio.provider == GlobalVariable.loginUser).ToCollectionAsync();
                PostedCommentsItems = await PostedCommentsTable. Where(Comments => Comments.from == GlobalVariable.loginUser).ToCollectionAsync();
                ReceivedCommentsItems = await ReceivedCommentsTable.Where(Comments => Comments.to == GlobalVariable.loginUser).ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                int length = TextbookItems.Count();
                for (int count = 0; count < length; count++)
                    ListTextbook.Items.Add(TextbookItems[count].bookName);
                length = JobItems.Count();
                for (int count = 0; count < length; count++)
                    ListJobs.Items.Add(JobItems[count].title);
                length = RentRoomItems.Count();
                for (int count = 0; count < length; count++)
                    ListRoom.Items.Add(RentRoomItems[count].roomTitle);
                length = SocietyItems.Count();
                for (int count = 0; count < length; count++)
                    ListClub.Items.Add(SocietyItems[count].societyName);
                length = PartyItems.Count();
                for (int count = 0; count < length; count++)
                    ListParty.Items.Add(PartyItems[count].partyTitle);
                length = StuffItems.Count();
                for (int count = 0; count < length; count++)
                    ListStuff.Items.Add(StuffItems[count].name);
                length = SharedAudioItems.Count();
                for (int count = 0; count < length; count++)
                    ListSharedRecord.Items.Add(SharedAudioItems[count].fileName);
                length = ReceivedCommentsItems.Count();
                for (int count = 0; count < length; count++)
                    ListReceivedReview.Items.Add(ReceivedCommentsItems[count].title);
                length = PostedCommentsItems.Count();
                for (int count = 0; count < length; count++)
                    ListPostedReview.Items.Add(PostedCommentsItems[count].title);

            }
            try
            {
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
                for (int count = 0; count < blobs.Count; count++)
                {
                    string name = blobs[count].Uri.ToString().Replace(
                         "https://uonlife.blob.core.windows.net/" +
                         GlobalVariable.loginUser.ToLower() + "/", "");
                    ListPrivateRecord.Items.Add(name);
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error loading items").ShowAsync();
            }
            Ring.IsActive = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void ListSharedRecord_ItemClick(object sender, ItemClickEventArgs e)
        {
            string item = e.ClickedItem.ToString();
            Frame.Navigate(typeof(DownloadRecordInfo), item);
        }

        private void ListPrivateRecord_ItemClick(object sender, ItemClickEventArgs e)
        {
            string item = e.ClickedItem.ToString();
            Frame.Navigate(typeof(DownloadRecordInfo), item);
        }

        private void ListStuff_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(ContentStuff), itemName);
        }

        private void ListJobs_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(ContentJob), itemName);
        }

        private void ListParty_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(ContentParty), itemName);
        }

        private void ListClub_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(ContentSociety), itemName);
        }

        private void ListRoom_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(ContengRentRoom), itemName);
        }

        private void ListTextbook_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(ContentTextbook), itemName);
        }

        private void ListReceivedReview_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(Comment), itemName);
        }

        private void ListPostedReview_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(Comment), itemName);
        }
    }
}
