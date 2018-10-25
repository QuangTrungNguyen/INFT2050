using UONlife.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;
using Windows.UI.Popups;
using UONlife.ReviewSystem;

namespace UONlife.Content
{

    public sealed partial class ContentJob : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Collection
        // Collection variables
        private MobileServiceCollection<Job, Job> items;
        private IMobileServiceTable<Job> jobTable = App.mobileService.GetTable<Job>();

        string callNumber;
        public ContentJob()
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


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            ContentJob data = e.Parameter as ContentJob;
            string title = (string)e.Parameter;
            lblTitle.Text = title;
            // Load data
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await jobTable
                    .Where(Job => Job.title == title)
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
                lblSalary.Text = items[0].salary;
                lblType.Text = items[0].type;
                lblRequirements.Text = items[0].requirements;
                lblClassification.Text = items[0].classification;
                lblWorkingPlace.Text = items[0].workingPlace;
                lblDescription.Text = items[0].description;
                lblContact.Text = items[0].contact;
                tbxPublisher.Text = items[0].publisher;
            }
            Ring.IsActive = false;
            // Get number from contact
            GlobalMethod global = new GlobalMethod();
            callNumber = global.getNumber(items[0].contact);
            btnCall.Content = "Call " + callNumber;
            btnSMS.Content = "SMS " + callNumber;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            if (callNumber != null)
                Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(callNumber, "Employer/Vendor");
        }

        private async void btnSMS_Click(object sender, RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                msg.Body = "I am seek for this job: " + items[0].title;
                msg.Recipients.Add(callNumber);
                await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
            }
        }

        private void btnCheckReview_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CommentList), tbxPublisher.Text);
        }

        private void btnPostReview_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PostComment), tbxPublisher.Text);
        }
    }
}
