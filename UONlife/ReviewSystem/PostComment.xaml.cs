using UONlife.Common;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using UONlife.DataModel;

namespace UONlife.ReviewSystem
{
    public sealed partial class PostComment : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PostComment()
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
            string to = (string)e.Parameter;
            lblTo.Text = to;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private async void btnPost_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (rbtGood.IsChecked == false && rbtGeneral.IsChecked == false && rbtBad.IsChecked == false)
                await new MessageDialog("Please select a class").ShowAsync();
            else if (tbxTitle.Text.Trim() == "")
                await new MessageDialog("Please write a title of your comment").ShowAsync();
            else if (tbxComment.Text.Trim() == "")
                await new MessageDialog("Please write comment").ShowAsync();
            else
            {
                string myLevel;
                if (rbtBad.IsChecked == true)
                    myLevel = "bad";
                else if (rbtGood.IsChecked == true)
                    myLevel = "good";
                else
                    myLevel = "general";
                try
                {
                    btnPost.IsEnabled = false;
                    btnPost.Content = "Uploading......";
                    Comments comments = new Comments
                    {
                        from = GlobalVariable.loginUser,
                        to = lblTo.Text,
                        level = myLevel,
                        title = tbxTitle.Text,
                        comment = tbxComment.Text
                    };
                    await App.mobileService.GetTable<Comments>().InsertAsync(comments);
                    MessageDialog msgbox = new MessageDialog("Upload success");
                    await msgbox.ShowAsync();
                    Frame.Navigate(typeof(MainPage));
                    btnPost.IsEnabled = true;
                    btnPost.Content = "Post Comment";
                }
                catch (Exception ex)
                {
                    await new MessageDialog("Failed to post comment: " + ex.Message).ShowAsync();
                }
            }
        }
    }
}
