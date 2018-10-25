using UONlife.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UONlife
{

    public sealed partial class Login : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Collection variables
        private MobileServiceCollection<Account, Account> items;
        private IMobileServiceTable<Account> accountTable = App.mobileService.GetTable<Account>();

        public Login()
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Register));
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbxUsername.Text.Trim() == "")
                await new MessageDialog("Username cannot be blank").ShowAsync();
            else if (tbxPassword.Password == "")
                await new MessageDialog("Password cannot be blank").ShowAsync();
            else
            {
                // Disable login button
                btnLogin.IsEnabled = false;
                btnLogin.Content = "Logging in ......";
                // Load data
                MobileServiceInvalidOperationException exception = null;
                try
                {
                    items = await accountTable
                        .Where(Account => Account.Username == tbxUsername.Text.Trim())
                        .ToCollectionAsync();
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    exception = ex;
                }
                if (exception != null)
                {
                    btnLogin.IsEnabled = true;
                    btnLogin.Content = "Log in";
                    await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
                }
                else
                {
                    btnLogin.IsEnabled = true;
                    btnLogin.Content = "Log in";
                    if (items.Count == 0)
                        await new MessageDialog("User not exist").ShowAsync();
                    else 
                    {
                        if (tbxPassword.Password != items[0].Password)
                            await new MessageDialog("Incorrect password, please try again").ShowAsync();
                        else
                        {
                            // Store current user
                            GlobalVariable.loginUser = tbxUsername.Text.Trim().ToLower();
                            Frame.Navigate(typeof(MainPage));            
                        }
                    }
                }
            }
        }
    }
}
