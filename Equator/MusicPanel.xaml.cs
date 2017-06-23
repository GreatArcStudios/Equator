using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BlendModeEffectLibrary;
using Equator.Controls;
using Equator.Helpers;
using Equator.Helpers.Background;
using Equator.Music;
using MahApps.Metro.Controls;
using CefSharp;
using System.IO;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MusicPanel.xaml
    /// </summary>
    public partial class MusicPanel : MetroWindow
    {
        public static bool IsPlaying;

        private static int _index;
        private readonly BackgroundEffectBehavior _backgroundEffectBehavior = new BackgroundEffectBehavior();
        private BehaviorCollection _behaviorCollection;
        private MusicCards _musicCards;
        private bool sliderDragging = false;
        private bool isReplay = false;
        private string songURI;
        public MusicPanel()
        {
            InitializeComponent();
            var userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
            userImageBrush.TileMode = TileMode.None;
            Userbutton.Background = userImageBrush;
            DispatcherTimer PlayTimer = new DispatcherTimer();
            PlayTimer.Interval = TimeSpan.FromSeconds(1);
            PlayTimer.Tick += new EventHandler(timer_Tick);
            PlayTimer.Start();
            
            var backgroundImageBrush = new ImageBrush(
                    new BitmapImage(
                        new Uri(FilePaths.DEFAULT_IMAGE_LOCATION)));
            backgroundImageBrush.Stretch = Stretch.UniformToFill;
            Background.Fill = backgroundImageBrush;
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            //Thanks for the baseplate 
            //http://www.wpf-tutorial.com/audio-video/how-to-creating-a-complete-audio-video-player/
            if ((mediaElement.Source != null) && (mediaElement.NaturalDuration.HasTimeSpan) && (!sliderDragging))
            {
                PlayBarSlider.Minimum = 0;
                PlayBarSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                PlayBarSlider.Value = mediaElement.Position.TotalSeconds;
            }
        }

        public static int GetIndex()
        {
            return _index;
        }

        public static void SetIndex(int indexParam)
        {
            _index = indexParam;
        }

        private void MusicPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            // ExtensionMethods.Refresh(TopBar);
        }

        private void MusicPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // RefreshTopBar();
            //RefreshBottomBar();
        }


        /* public void RefreshTopBar(Object state)
        {
            BlurEffect topBlurEffect = new BlurEffect();
            topBlurEffect.Radius = 100;
            TopBar.Effect = topBlurEffect;
        }  */
#if USING_SPECIAL_EFFECTS
        //not used
        public void RefreshTopBar()
        {
            _behaviorCollection = Interaction.GetBehaviors(TopBar);
            var overlayEffect = new OverlayEffect();
            var geometryDrawing = new GeometryDrawing();
            geometryDrawing.Geometry = new RectangleGeometry(new System.Windows.Rect(0, 0, 1, 1));
            var color = new Color();
            color.A = 255;
            color.R = 88;
            color.G = 88;
            color.B = 88;
            geometryDrawing.Brush = new SolidColorBrush(color);
            var drawingImage = new DrawingImage();
            drawingImage.Drawing = geometryDrawing;
            var imageBrush = new ImageBrush();
            imageBrush.ImageSource = drawingImage;
            overlayEffect.BInput = imageBrush;

            BlendModeEffect blendModeEffect = overlayEffect;

            // _behaviorCollection.RemoveAt(0);
            _backgroundEffectBehavior.Visual = TopBar;
            _backgroundEffectBehavior.Effect = blendModeEffect;
            _behaviorCollection.Add(_backgroundEffectBehavior);
            Console.WriteLine("resized");
        }
        //not used
        public void RefreshBottomBar(object state)
        {
            var lightenEffect = new LightenEffect();
            PlayBar.Effect = lightenEffect;
            Console.WriteLine("refreshed");
        }
        //not used
        public void RefreshBottomBar()
        {
            var behaviorCollection = Interaction.GetBehaviors(PlayBar);
            //_behaviorCollection.RemoveAt(0);
            var backgroundEffectBehavior = new BackgroundEffectBehavior();
            backgroundEffectBehavior.Visual = PlayBar;
            var overlayEffect = new OverlayEffect();
            var geometryDrawing = new GeometryDrawing();
            geometryDrawing.Geometry = new RectangleGeometry();
            var color = new Color();
            color.A = 255;
            color.R = 145;
            color.G = 148;
            color.B = 149;
            geometryDrawing.Brush = new SolidColorBrush(color);
            var drawingImage = new DrawingImage();
            drawingImage.Drawing = geometryDrawing;
            var imageBrush = new ImageBrush();
            imageBrush.ImageSource = drawingImage;
            overlayEffect.BInput = imageBrush;
            BlendModeEffect blendModeEffect = new OverlayEffect();
            ;
            backgroundEffectBehavior.Effect = blendModeEffect;
            behaviorCollection.Add(backgroundEffectBehavior);

            Console.WriteLine("resized");
        }
#endif
        //TODO: make it so that topic videos don't play
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(BoredLabel, -9999999);
            MusicContainer.Children.RemoveRange(0, MusicContainer.Children.Count);
            QueryVideo.QueryList(SearchBox.Text);
            //add card add logic here
            for (var i = 0; i < QueryVideo.SongCount; i++)
            {
                var artistName = QueryVideo.SearchListResponse.Items[i].Snippet.ChannelTitle;
                if (artistName.Contains("VEVO"))
                {
                    artistName.Replace("VEVO", "");
                }
                _musicCards = new MusicCards(QueryVideo.SearchListResponse.Items[i].Id.VideoId,
                    QueryVideo.SearchListResponse.Items[i].Snippet.Title, artistName,
                    new Uri(QueryVideo.SearchListResponse.Items[i].Snippet.Thumbnails.Medium.Url), ref mediaElement,
                    ref CurrentSong, ref EndTimeLabel, ref Background, ref PlayBarSlider, i, ref media.CefPlayer);
                MusicContainer.Children.Add(_musicCards);

            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchButton_Click(this, new RoutedEventArgs());
        }

        private void Userbutton_Click(object sender, RoutedEventArgs e)
        {
            var msgBoxResult = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (msgBoxResult == MessageBoxResult.Yes)
            {
                GoogleServices.LogOut();

                var window = new MainWindow();

                Close();
                window.Show();
            }
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
            {
                //mediaElement.Pause();
                //mediaElement.Opacity = 0;
                string script = "var youtubePlayer = document.getElementById(\"youtubePlayer\");";
                script += "youtubePlayer.play();";
                media.CefPlayer.ExecuteScriptAsync(script);
                IsPlaying = false;
            }
            else
            {
                string script = "var youtubePlayer = document.getElementById(\"youtubePlayer\");";
                script += "youtubePlayer.play();";
                media.CefPlayer.ExecuteScriptAsync(script);
                //mediaElement.Play();
                ///mediaElement.Opacity = 100;
                IsPlaying = true;
            }
        }

        private async void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            BoredLabel.IsEnabled = false;
            SetIndex(_index + 1);
            //make it play the first song
            if (GetIndex() == MusicContainer.Children.Count - 1)
            {
                // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                await
              GetSong.PlaySpecifiedSong(Background, mediaElement,
                  QueryVideo.SearchListResponse.Items[0].Id.VideoId,
                  _index,
                  QueryVideo.SearchListResponse.Items[0].Snippet.Title,
                  CurrentSong, media.CefPlayer);
            }
            //otherwise play the next song
            await GetSong.PlaySpecifiedSong(Background, mediaElement,
                 QueryVideo.SearchListResponse.Items[_index].Id.VideoId,
                 _index,
                 QueryVideo.SearchListResponse.Items[_index].Snippet.Title,
                 CurrentSong, media.CefPlayer);
        }

        private void PanelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                if (IsPlaying)
                {
                    mediaElement.Pause();
                    IsPlaying = false;
                }
                else
                {
                    mediaElement.Play();
                    IsPlaying = true;
                }
        }

        private void PlayBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            PlayBarSlider = (Slider)sender;
            sliderDragging = true;
        }
        private void PlayBar_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            PlayBarSlider = (Slider)sender;
            sliderDragging = false;
            mediaElement.Position = TimeSpan.FromSeconds(PlayBarSlider.Value);
        }
        private void PlayBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayBarSlider = (Slider)sender;
            CurrentTimeLabel.Content = TimeSpan.FromSeconds(PlayBarSlider.Value).ToString(@"mm\:ss");
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            EndTimeLabel.Content = TimeSpan.FromSeconds(mediaElement.NaturalDuration.TimeSpan.TotalSeconds).ToString(@"mm\:ss");
            songURI = mediaElement.Source.AbsoluteUri;
        }

        private async void GoLeftButtonClick(object sender, RoutedEventArgs e)
        {
            SetIndex(_index - 1);
            //make it play the first song
            if (GetIndex() == 0)
            {
                // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                await
              GetSong.PlaySpecifiedSong(Background, mediaElement,
                  QueryVideo.SearchListResponse.Items[QueryVideo.SearchListResponse.Items.Count - 1].Id.VideoId,
                  _index,
                  QueryVideo.SearchListResponse.Items[QueryVideo.SearchListResponse.Items.Count - 1].Snippet.Title,
                  CurrentSong, media.CefPlayer);
            }
            //otherwise play the next song
            await GetSong.PlaySpecifiedSong(Background, mediaElement,
                 QueryVideo.SearchListResponse.Items[_index].Id.VideoId,
                 _index,
                 QueryVideo.SearchListResponse.Items[_index].Snippet.Title,
                 CurrentSong, media.CefPlayer);
        }
        /*
        private async void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            mediaElement.Opacity = 0;
            if (!isReplay)
                await GetSong.AutoPlaySong(_index + 1, CurrentSong, MusicContainer, mediaElement, Background, );
            else
                mediaElement.Source = new Uri(songURI);
            mediaElement.Play();
        }*/

        private void Replay_Button_Click(object sender, RoutedEventArgs e)
        {
            if (isReplay)
            {
                isReplay = false;
            }
            isReplay = true;
        }
    }

}
