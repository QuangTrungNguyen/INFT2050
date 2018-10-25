using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using UONlife.Common;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace UONlife.Study
{
    public sealed partial class Recorder : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private MediaCapture _mediaCaptureManager;
        private StorageFile _recordStorageFile;
        private bool _recording;
        private bool _userRequestedRaw;
        private bool _rawAudioSupported;
        MediaEncodingProfile recordProfile;
        String fileName;

        private bool playing = false;
        public Recorder()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            InitializeAudioRecording();
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
        private async void InitializeAudioRecording()
        {

            _mediaCaptureManager = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings();
            settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
            settings.MediaCategory = MediaCategory.Other;
            settings.AudioProcessing = (_rawAudioSupported && _userRequestedRaw) ? AudioProcessing.Raw : AudioProcessing.Default;

            await _mediaCaptureManager.InitializeAsync(settings);


            _mediaCaptureManager.RecordLimitationExceeded += new RecordLimitationExceededEventHandler(RecordLimitationExceeded);
            _mediaCaptureManager.Failed += new MediaCaptureFailedEventHandler(Failed);
        }

        private void Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            throw new NotImplementedException();
        }

        private void RecordLimitationExceeded(MediaCapture sender)
        {
            throw new NotImplementedException();
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (tbxCourseID.Text.Trim() == "" || tbxWeek.Text.Trim() == "")
            {
                await new MessageDialog("Please enter in a valid course ID and week number").ShowAsync();
            }
            else
            {
                try
                {
                    Random r = new Random();
                    CaptureButton.IsEnabled = false;
                    CaptureButton.Content = "Recording......";
                    StopCaptureButton.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    PlayRecordButton.IsEnabled = false;
                    fileName = tbxCourseID.Text.Trim() + "Week" +
                                      tbxWeek.Text.Trim() + "No" +
                                      r.Next(1000, 9999).ToString() + ".m4a";
                    // Record audio
                    // Source: http://stackoverflow.com/questions/23048661/capture-and-playback-of-audio-wp8-1-xaml
                    _recordStorageFile = await KnownFolders.VideosLibrary.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                    recordProfile = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto);
                    await _mediaCaptureManager.StartRecordToStorageFileAsync(recordProfile, this._recordStorageFile);
                    _recording = true;
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, "Error").ShowAsync();
                }
            }
        }

        private async void StopCaptureButton_Click(object sender, RoutedEventArgs e)
        {      
            if (_recording)
            {
                await _mediaCaptureManager.StopRecordAsync();
                _recording = false;
                CaptureButton.IsEnabled = true;
                CaptureButton.Content = "Capture Audio";
                StopCaptureButton.IsEnabled = false;
                btnSave.IsEnabled = true;
                PlayRecordButton.IsEnabled = true;
                await new MessageDialog("Audio file stored, please check your music library.").ShowAsync();
            }
        }

        private async void PlayRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (playing == false)
            {
                if (!_recording)
                {
                    PlayRecordButton.Content = "Stop Playing";
                    playing = true;
                    var stream = await _recordStorageFile.OpenAsync(FileAccessMode.Read);
                    playbackElement1.AutoPlay = true;
                    playbackElement1.SetSource(stream, _recordStorageFile.FileType);
                    playbackElement1.Play();
                }
            }
            else
            {
                playbackElement1.Stop();
                playbackElement1.Pause();
                PlayRecordButton.Content = "Play Capture";
                playing = false;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSave.IsEnabled = false;
                btnSave.Content = "Uploading....";
                progress.Visibility = Visibility.Visible;
                // Source: https://azure.microsoft.com/en-us/documentation/articles/mobile-services-javascript-backend-windows-universal-dotnet-upload-data-blob-storage/#test
                // Part one: upload files to database
                // Create the connectionstring
                String StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=uonlife;AccountKey=LzU9gRoJgvtKtY7rIPE3w1Z7Toc39AfcBO+Y+Q4ZCYoZmXd2KTgpZ5muya6JkxaZRtNAo3ib3FTpw7gAncpOPA==";
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container. (pictures)
                CloudBlobContainer container = blobClient.GetContainerReference(GlobalVariable.loginUser.ToLower());
                await container.CreateIfNotExistsAsync();
                
                string sFileName = recordProfile.ToString();
                CloudBlockBlob blobFromSASCredential =
                                  container.GetBlockBlobReference(fileName);
                await blobFromSASCredential.UploadFromFileAsync(_recordStorageFile);

                MessageDialog msgbox = new MessageDialog("Upload success");
                await msgbox.ShowAsync();

                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                MessageDialog msgbox = new MessageDialog("Error: " + ex.Message);
                await msgbox.ShowAsync();
                btnSave.IsEnabled = true;
                btnSave.Content = "Upload File To Cloud";
                progress.Visibility = Visibility.Collapsed;
            }
        }

    }
}
