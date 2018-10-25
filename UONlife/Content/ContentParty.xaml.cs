using UONlife.Common;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Microsoft.WindowsAzure.MobileServices;
using UONlife.DataModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Documents;
using Windows.UI;
using UONlife.ReviewSystem;

namespace UONlife.Content
{
    public sealed partial class ContentParty : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // Maps
        Geolocator geolocator = new Geolocator();
        Geoposition geoposition = null;
        MapLocationFinderResult result;

        // Collection variables
        private MobileServiceCollection<Party, Party> items;
        private IMobileServiceTable<Party> partyTable = App.mobileService.GetTable<Party>();
        string callNumber;
        public ContentParty()
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
            ContengRentRoom data = e.Parameter as ContengRentRoom;
            string partyTitle = (string)e.Parameter;
            lblTitle.Text = partyTitle;
            // Load data
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await partyTable
                    .Where(Party => Party.partyTitle == partyTitle)
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
                lblTitle.Text = items[0].partyTitle;
                lblTime.Text = items[0].time;
                lblContact.Text = items[0].contact;
                lblContent.Text = items[0].content;
                lblCrowd.Text = items[0].crowd;
                lblPrice.Text = items[0].price;
                tbxPublisher.Text = items[0].publisher;
                lblAddress.Text = items[0].address;
                try
                {
                    // Download image from blob
                    // Souece: http://stackoverflow.com/questions/23256372/windows-phone-8-1-loading-images-from-remote-urls-placeholder
                    Uri myUri = new Uri(items[0].imageAddress, UriKind.Absolute);
                    BitmapImage bmi = new BitmapImage();
                    bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmi.UriSource = myUri;
                    img.Source = bmi;

                    // Show room location on map
                    // Authorization of map services
                    map.MapServiceToken = "QRJj0BZ57yT77gTeYfN-uw";

                    // Show current location
                    // Source: https://msdn.microsoft.com/en-us/magazine/dn818495.aspx    

                    geolocator = new Geolocator();
                    geoposition = await geolocator.GetGeopositionAsync();
                    result = await MapLocationFinder.FindLocationsAsync(items[0].address, geoposition.Coordinate.Point, 1);
                    map.Center = result.Locations[0].Point;
                    map.ZoomLevel = 15;

                    //Pin image
                    MapIcon mapIcon = new MapIcon();
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MapPin.png"));
                    mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
                    mapIcon.Location = result.Locations[0].Point;
                    mapIcon.Title = "Room is here";
                    map.MapElements.Add(mapIcon);
                }
                catch (Exception ex)
                {
                    MessageDialog msgbox = new MessageDialog("Failed to acquire information: " + ex.Message);
                    await msgbox.ShowAsync();
                }
            }
            Ring.IsActive = false;
            // Get number from contact
            GlobalMethod global = new GlobalMethod();
            callNumber = global.getNumber(items[0].contact);
            btnCall.Content = "Call " + callNumber;
            btnSMS.Content = "SMS " + callNumber;

            // Get the drive rout between two points
            // Source: https://msdn.microsoft.com/en-us/magazine/dn818495.aspx
            try
            {
                BasicGeoposition startLocation = new BasicGeoposition();
                startLocation.Altitude = geoposition.Coordinate.Point.Position.Altitude;
                startLocation.Latitude = geoposition.Coordinate.Point.Position.Latitude;
                startLocation.Longitude = geoposition.Coordinate.Point.Position.Longitude;
                Geopoint startPoint = new Geopoint(startLocation);
                BasicGeoposition endLocation = new BasicGeoposition();
                endLocation.Altitude = result.Locations[0].Point.Position.Altitude;
                endLocation.Latitude = result.Locations[0].Point.Position.Latitude;
                endLocation.Longitude = result.Locations[0].Point.Position.Longitude;
                Geopoint endPoint = new Geopoint(endLocation);
                // Get the route between the points.
                MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                                                   startPoint, endPoint,
                                                   MapRouteOptimization.Time, MapRouteRestrictions.None, 290);
                // Display the route between two points
                if (routeResult.Status == MapRouteFinderStatus.Success)
                {
                    // Use the route to initialize a MapRouteView.
                    MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                    viewOfRoute.RouteColor = Colors.Blue;
                    viewOfRoute.OutlineColor = Colors.Blue;
                    // Add the new MapRouteView to the Routes collection
                    // of the MapControl.
                    map.Routes.Add(viewOfRoute);
                    // Fit the MapControl to the route.
                    await map.TrySetViewBoundsAsync(
                      routeResult.Route.BoundingBox,
                      null,
                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                }
                // Display summary info about the route.
                // Source: https://msdn.microsoft.com/en-us/magazine/dn818495.aspx
                route.Inlines.Add(new Run()
                {
                    Text = "Total estimated time (minutes) = " + routeResult.Route.EstimatedDuration.TotalMinutes.ToString("F1")
                });
                route.Inlines.Add(new LineBreak());
                route.Inlines.Add(new Run()
                {
                    Text = "Total length (kilometers) = "
                    + (routeResult.Route.LengthInMeters / 1000).ToString("F1")
                });
                route.Inlines.Add(new LineBreak());
                // Display the directions.
                route.Inlines.Add(new Run()
                {
                    Text = "DIRECTIONS"
                });
                route.Inlines.Add(new LineBreak());
                route.Inlines.Add(new LineBreak());
                // Loop through the legs and maneuvers.

                foreach (MapRouteLeg leg in routeResult.Route.Legs)
                {
                    foreach (MapRouteManeuver maneuver in leg.Maneuvers)
                    {
                        route.Inlines.Add(new Run()
                        {
                            Text = maneuver.InstructionText
                        });
                        route.Inlines.Add(new LineBreak());
                    }
                }
            }
            catch
            {
                await new MessageDialog("No route from your location to party location.").ShowAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(callNumber, "Landlord");
            }
        }

        private async void btnSMS_Click(object sender, RoutedEventArgs e)
        {
            if (callNumber != null)
            {
                Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                msg.Body = "I wound like attend this party: " + items[0].address;
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

        private void abbMap_Click(object sender, RoutedEventArgs e)
        {
            map.Visibility = Windows.UI.Xaml.Visibility.Visible;
            route.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            abbMap.IsEnabled = false;
            abbDirections.IsEnabled = true;
        }

        private void abbDirections_Click(object sender, RoutedEventArgs e)
        {
            map.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            route.Visibility = Windows.UI.Xaml.Visibility.Visible;
            abbMap.IsEnabled = true;
            abbDirections.IsEnabled = false;
        }
    }
}
