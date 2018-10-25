using UONlife.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;
using Windows.UI.Popups;

namespace UONlife.ReviewSystem
{
    public sealed partial class Comment : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Collection
        // Collection variables
        private MobileServiceCollection<Comments, Comments> items;
        private IMobileServiceTable<Comments> commentsTable = App.mobileService.GetTable<Comments>();
        public Comment()
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
            Comment data = e.Parameter as Comment;
            string title = (string)e.Parameter;
            lblTitle.Text = title;

            // Load data
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await commentsTable
                    .Where(Comments => Comments.title == title)
                    .ToCollectionAsync();
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
                // Show data
                lblTitle.Text = items[0].title;
                lblFrom.Text = items[0].from;
                tbxComment.Text = items[0].comment;
                lblLevel.Text = items[0].level;
            }
            Ring.IsActive = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
    }
}
