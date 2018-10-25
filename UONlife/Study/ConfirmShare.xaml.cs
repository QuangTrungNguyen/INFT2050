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
using UONlife.DataModel;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UONlife.Study
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfirmShare : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ConfirmShare()
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
            lblAddress.Text = "https://uonlife.blob.core.windows.net/" +
                               GlobalVariable.loginUser.ToLower() + "/" +
                               title;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private async void btnShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnShare.IsEnabled = false;
                btnShare.Content = "Sharing......";
                //store data into table
                SharedAudio sharedAudio = new SharedAudio
                {
                    fileName = lblName.Text,
                    uri = lblAddress.Text,
                    provider = GlobalVariable.loginUser
                };
                await App.mobileService.GetTable<SharedAudio>().InsertAsync(sharedAudio);
                MessageDialog msgbox = new MessageDialog("Sharing success");
                await msgbox.ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                MessageDialog msgbox = new MessageDialog("Error: " + ex.Message);
                await msgbox.ShowAsync();
                btnShare.IsEnabled = true;
                btnShare.Content = "Share";
            }
        }
    }
}
