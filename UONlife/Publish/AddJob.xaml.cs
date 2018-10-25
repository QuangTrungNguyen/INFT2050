using UONlife.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UONlife.DataModel;
using Windows.UI.Popups;

namespace UONlife.Publish
{

    public sealed partial class AddJob : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AddJob()
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

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (tbxTitle.Text.Trim() == "")
            {
                MessageDialog msgbox = new MessageDialog("You must provide a valid job title");
                await msgbox.ShowAsync();
            }
            else
            {
                try
                {
                    btnUpload.IsEnabled = false;
                    btnUpload.Content = "Uploading......";
                    //store data into table
                   
                    Job job = new Job
                    {
                        title = tbxTitle.Text,
                        type = tbxType.Text,
                        classification = tbxClassification.Text,
                        salary = tbxSalary.Text,
                        workingPlace = tbxWorkingPlace.Text,
                        description = tbxDescription.Text,
                        requirements = tbxRequirements.Text,
                        contact = tbxContact.Text,
                        publisher = GlobalVariable.loginUser
                    };
                    await App.mobileService.GetTable<Job>().InsertAsync(job);
                    MessageDialog msgbox = new MessageDialog("Upload success");
                    await msgbox.ShowAsync();
                    Frame.Navigate(typeof(MainPage));
                }
                catch (Exception ex)
                {
                    MessageDialog msgbox = new MessageDialog("Error: " + ex.Message);
                    await msgbox.ShowAsync();
                    btnUpload.IsEnabled = true;
                    btnUpload.Content = "Upload";
                }
            }
        }
    }
}
