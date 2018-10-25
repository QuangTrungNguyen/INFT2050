using UONlife.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using UONlife.ReviewSystem;

namespace UONlife.Content
{

    public sealed partial class ContentStuff : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Collection variables
        private MobileServiceCollection<Stuff, Stuff> items;
        private IMobileServiceTable<Stuff> stuffTable = App.mobileService.GetTable<Stuff>();
        string callNumber;

        public ContentStuff()
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
            this.navigationHelper.OnNavigatedTo(e);
            ContentSociety data = e.Parameter as ContentSociety;
            string name = (string)e.Parameter;
            lblName.Text = name;
            // Load data
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await stuffTable
                    .Where(Stuff => Stuff.name == name)
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
                lblPrice.Text = items[0].description;
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

        private void btnPostReview_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PostComment), tbxPublisher.Text);
        }

        private void btnCheckReview_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CommentList), tbxPublisher.Text);
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(callNumber, "Society Admin");
            }
        }

        private async void btnSMS_Click(object sender, RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                msg.Body = "I wound like to buy: " + items[0].name;
                msg.Recipients.Add(callNumber);
                await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
            }
        }
    }
}
