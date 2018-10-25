using Microsoft.WindowsAzure.MobileServices;
using System;
using UONlife.Common;
using UONlife.DataModel;
using UONlife.ReviewSystem;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;



namespace UONlife.Content
{

    public sealed partial class ContentTextbook : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        // Collection variables
        private MobileServiceCollection<Textbook, Textbook> items;
        private IMobileServiceTable<Textbook> textbookTable = App.mobileService.GetTable<Textbook>();

        string callNumber;
        public ContentTextbook()
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
            ContentTextbook data = e.Parameter as ContentTextbook;
            string bookName = (string)e.Parameter;
            lblBookname.Text = bookName;
            // Load data
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await textbookTable
                    .Where(Textbook => Textbook.bookName == bookName)
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
                lblCourseID.Text = items[0].courseID;
                lblDepreciation.Text = items[0].depreciation;
                lblPrice.Text = items[0].price;
                lblDescription.Text = items[0].description;
                lblContact.Text = items[0].contact;
                tbxPublisher.Text = items[0].publisher;
                try
                {
                    // Download image from blob
                    // Souece: http://stackoverflow.com/questions/23256372/windows-phone-8-1-loading-images-from-remote-urls-placeholder
                    Uri myUri = new Uri(items[0].imageAddress, UriKind.Absolute);
                    BitmapImage bmi = new BitmapImage();
                    bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmi.UriSource = myUri;
                    img.Source = bmi;
                }
                catch (Exception ex)
                {
                    MessageDialog msgbox = new MessageDialog("Failed to acquire picture: " + ex.Message);
                    await msgbox.ShowAsync();
                }
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

        private void btnCall_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(callNumber, "Book Seller");
            }
        }

        private async void btnSMS_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                msg.Body = "I wound like to buy your book: " + items[0].bookName;
                msg.Recipients.Add(callNumber);
                await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
            }
        }

        private void btnCheckReview_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CommentList), tbxPublisher.Text);
        }

        private void btnPostReview_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PostComment), tbxPublisher.Text);
        }
    }
}
