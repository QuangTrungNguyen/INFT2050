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
using Windows.UI.Popups;
using UONlife.DataModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UONlife
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Register()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Check information
            if (tbxUsername.Text.Trim() == "")
            {
                MessageDialog msgbox = new MessageDialog("You must provide a valid username");
                await msgbox.ShowAsync();
            }
            else if (tbxEmail.Text.Trim() == "")
            {
                MessageDialog msgbox = new MessageDialog("You must provide a valid email address");
                await msgbox.ShowAsync();
            }
            else if (tbxPassword.Password == "")
            {
                MessageDialog msgbox = new MessageDialog("You must provide a valid password");
                await msgbox.ShowAsync();
            }
            else if (tbxPassword.Password != tbxRepeatPassword.Password)
            {
                MessageDialog msgbox = new MessageDialog("The two passwords you typed do not match.");
                await msgbox.ShowAsync();
            }
            else
            {
                btnRegister.IsEnabled = false;
                btnRegister.Content = "Processing ......";
                Account account = new Account
                {
                    Username = tbxUsername.Text.ToLower(),
                    Password = tbxPassword.Password,
                    Email = tbxEmail.Text,
                    StorageSpace = 100
                };
                try
                {
                    await App.mobileService.GetTable<Account>().InsertAsync(account);
                   
                    // Create a personal container
                    // Create the connectionstring
                    String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
                    // Retrieve storage account from connection string.
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                    // Create the blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    //the container for this is companystyles
                    CloudBlobContainer container = blobClient.GetContainerReference(tbxUsername.Text.Trim().ToLower());
                    await container.CreateIfNotExistsAsync();
                    BlobContainerPermissions permission = new BlobContainerPermissions();
                    permission.PublicAccess = BlobContainerPublicAccessType.Blob;
                    await container.SetPermissionsAsync(permission);
                    MessageDialog msgbox = new MessageDialog("Success, please login.");
                    await msgbox.ShowAsync();
                    Frame.Navigate(typeof(Login));
                }
                catch (Exception ex)
                {
                    MessageDialog msgbox = new MessageDialog("Error: " + ex.Message);
                    await msgbox.ShowAsync();
                    btnRegister.IsEnabled = true;
                    btnRegister.Content = "Register";
                }

            }
        }
    }
}
