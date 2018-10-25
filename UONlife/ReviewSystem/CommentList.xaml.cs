using UONlife.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;
using Windows.UI.Popups;

namespace UONlife.ReviewSystem
{

    public sealed partial class CommentList : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MobileServiceCollection<Comments, Comments> CommentsItems;
        private IMobileServiceTable<Comments> CommentsTable = App.mobileService.GetTable<Comments>();

        public CommentList()
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

            MobileServiceInvalidOperationException exception = null;
            try
            {
                CommentsItems = await CommentsTable.
                    Where(Comments => Comments.to == title).
                    ToCollectionAsync();
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
                int length = CommentsItems.Count();
                for (int count = 0; count < length; count++)
                    ListItems.Items.Add(CommentsItems[count].title);
            }
            Ring.IsActive = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            
        }

        private void ListItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            string itemName = e.ClickedItem.ToString();
            Frame.Navigate(typeof(Comment), itemName);
        }
    }
}
