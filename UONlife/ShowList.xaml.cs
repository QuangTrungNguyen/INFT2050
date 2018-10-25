using UONlife.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UONlife.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using UONlife.Content;


namespace UONlife
{

    public sealed partial class ShowList : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Collection variables           
        private MobileServiceCollection<Textbook, Textbook> textbookItems;
        private IMobileServiceTable<Textbook> textbookTable = App.mobileService.GetTable<Textbook>();

        private MobileServiceCollection<Job, Job> JobItems;
        private IMobileServiceTable<Job> JobTable = App.mobileService.GetTable<Job>();

        private MobileServiceCollection<RentRoom, RentRoom> RentRoomItems;
        private IMobileServiceTable<RentRoom> RentRoomTable = App.mobileService.GetTable<RentRoom>();

        private MobileServiceCollection<Society, Society> SocietyItems;
        private IMobileServiceTable<Society> SocietyTable = App.mobileService.GetTable<Society>();

        private MobileServiceCollection<Party, Party> PartyItems;
        private IMobileServiceTable<Party> PartyTable = App.mobileService.GetTable<Party>();

        private MobileServiceCollection<Stuff, Stuff> StuffItems;
        private IMobileServiceTable<Stuff> StuffTable = App.mobileService.GetTable<Stuff>();

        // Title passed from main page
        public ShowList()
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
            
            string title = (string)e.Parameter;
            lblTitle.Text = title;
            //Connect to azure table
            //Retrieve the storage account from the connection string.

            // Read data from table
            MobileServiceInvalidOperationException exception = null;        
            try
            {
                if (title == "Textbook")
                    textbookItems = await textbookTable.ToCollectionAsync();
                else if (title == "Job")
                    JobItems = await JobTable.ToCollectionAsync();
                else if (title == "Rent Room")
                    RentRoomItems = await RentRoomTable.ToCollectionAsync();
                else if (title == "Society & Club")
                    SocietyItems = await SocietyTable.ToCollectionAsync();
                else if (title == "Party")
                    PartyItems = await PartyTable.ToCollectionAsync();
                else if (title == "Stuff")
                    StuffItems = await StuffTable.ToCollectionAsync();
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
                // Add all data in a list
                if (title == "Textbook")
                {
                    int length = textbookItems.Count();
                    for (int count = 0; count < length; count++)
                        ListItems.Items.Add(textbookItems[count].bookName);
                }
                else if (title == "Job")
                {
                    int length = JobItems.Count();
                    for (int count = 0; count < length; count++)
                        ListItems.Items.Add(JobItems[count].title);
                }
                else if (title == "Rent Room")
                {
                    int length = RentRoomItems.Count();
                    for (int count = 0; count < length; count++)
                        ListItems.Items.Add(RentRoomItems[count].roomTitle);
                }
                else if (title == "Society & Club")
                {
                    int length = SocietyItems.Count();
                    for (int count = 0; count < length; count++)
                        ListItems.Items.Add(SocietyItems[count].societyName);
                }
                else if (title == "Party")
                {
                    int length = PartyItems.Count();
                    for (int count = 0; count < length; count++)
                        ListItems.Items.Add(PartyItems[count].partyTitle);
                }
                else if (title == "Stuff")
                {
                    int length = StuffItems.Count();
                    for (int count = 0; count < length; count++)
                        ListItems.Items.Add(StuffItems[count].name);
                }
            }      
            Ring.IsActive = false;           
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void ListItems_ItemClick(object sender, ItemClickEventArgs e)
        {

            // Get the name of selected items and passed information to content page
            string itemName = e.ClickedItem.ToString();
            if (lblTitle.Text == "Textbook")
                Frame.Navigate(typeof(ContentTextbook), itemName);
            else if (lblTitle.Text == "Job")
                Frame.Navigate(typeof(ContentJob), itemName);
            else if (lblTitle.Text == "Rent Room")
                Frame.Navigate(typeof(ContengRentRoom), itemName);
            else if (lblTitle.Text == "Society & Club")
                Frame.Navigate(typeof(ContentSociety), itemName);
            else if (lblTitle.Text == "Party")
                Frame.Navigate(typeof(ContentParty), itemName);
            else if (lblTitle.Text == "Stuff")
                Frame.Navigate(typeof(ContentStuff), itemName);
        }
    }
}
