using UONlife.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UONlife.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;

namespace UONlife.Study
{
    public sealed partial class SharedRecordList : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Collection variables           
        private MobileServiceCollection<SharedAudio, SharedAudio> SharedAudioItems;
        private IMobileServiceTable<SharedAudio> SharedAudioTable = App.mobileService.GetTable<SharedAudio>();

        public SharedRecordList()
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
            // Read data from table
            MobileServiceInvalidOperationException exception = null;
            try
            {
                SharedAudioItems = await SharedAudioTable.ToCollectionAsync();
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
                int length = SharedAudioItems.Count();
                for (int count = 0; count < length; count++)
                    ListItems.Items.Add(SharedAudioItems[count].fileName);
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
