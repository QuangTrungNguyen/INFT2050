using UONlife.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.FileProperties;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UONlife.Study
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UploadRecord : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public UploadRecord()
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
            IReadOnlyList<StorageFile> listMusicFiles = new List<StorageFile>();
            listMusicFiles = await KnownFolders.MusicLibrary.GetFilesAsync(CommonFileQuery.OrderByName);
            for (int i = 0; i < listMusicFiles.Count; i++)
            {
                MusicProperties musicProperties = await listMusicFiles[i].Properties.GetMusicPropertiesAsync();
                ListItems.Items.Add(musicProperties.Title);              
            }
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void ListItems_ItemClick(object sender, ItemClickEventArgs e)
        {

            string item = e.ClickedItem.ToString();
            Frame.Navigate(typeof(UploadRecordInfo), item);
        }
    }
}
