using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;
using CefSharp;
using Equator.Controls;
using Equator.Helpers;
using Equator.Music;
using Google.Apis.YouTube.v3.Data;
using MahApps.Metro.Controls;
using SuperfastBlur;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MusicPanel.xaml
    /// </summary>

    public partial class MusicPanel : MetroWindow
    {
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);

        public static bool IsPlaying;

        private static int _index;
        private MusicCards _musicCards;
        private bool _sliderDragging;
        private bool _isReplay;
        private bool _isShuffle;
        private bool _playingSongs = true;
        private System.Windows.Media.Color _selectedColor = System.Windows.Media.Color.FromArgb(127, 180, 180, 180);
        private System.Windows.Media.Color _deselectedColor = System.Windows.Media.Color.FromArgb(127, 126, 126, 126);
        private System.Windows.Media.Color _hoverColor = System.Windows.Media.Color.FromArgb(127, 170, 170, 170);
        private System.Windows.Media.Color _pressedColor = System.Windows.Media.Color.FromArgb(153, 60, 60, 60);

        //private string songURI;
        private const string CurrentTimeScript =
                "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); return youtubePlayer.currentTime;})();"
            ;

        private double _songDuration;
        private bool _songLoaded;
        private bool _firstSwitch = true;

        public MusicPanel()
        {
            InitializeComponent();
            //create user image
            ImageBrush userImageBrush;
            if (!File.Exists(FilePaths.SaveUserImage() + "\\Userimage.png"))
                userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
            else
            {
                userImageBrush = new ImageBrush(new BitmapImage(new Uri(FilePaths.SaveUserImage() + "\\Userimage.png")));
            }
            userImageBrush.TileMode = TileMode.None;
            Userbutton.Background = userImageBrush;
            //timer init 
            var playTimer = new DispatcherTimer();
            playTimer.Interval = TimeSpan.FromSeconds(1);
            playTimer.Tick += timer_Tick;
            playTimer.Start();

            //check and set default image to background
            if (File.Exists(FilePaths.DefaultImageLocation))
            {
                var backgroundImageBrush = new ImageBrush(new BitmapImage(new Uri(FilePaths.DefaultImageLocation)));
                backgroundImageBrush.Stretch = Stretch.UniformToFill;
                Background.Fill = backgroundImageBrush;
            }
            else
            {
                #region First run 
                var image = System.Drawing.Image.FromFile(new Uri(@"Images\Img_0535.jpg", UriKind.Relative).ToString());
                var blur = new GaussianBlur(image as Bitmap);
                var blurredThumb = blur.Process(70);
                image.Dispose();
                var hBitmap = blurredThumb.GetHbitmap();
                blurredThumb.Save(FilePaths.DefaultImageLocation, ImageFormat.Png);
                var backgroundImageBrush = new ImageBrush();
                backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
                DeleteObject(hBitmap);
                backgroundImageBrush.Stretch = Stretch.UniformToFill;
                Background.Fill = backgroundImageBrush;
                #endregion
            }
            //init media player
            media.CefPlayer.LoadingStateChanged += CefPlayer_LoadingStateChanged;
            media.Minimize.Visibility = Visibility.Hidden;
            //init trans content control
            //FullGrid.Children.Remove(SongSearchContainer);
            //TransitioningContentControl.Content = SongSearchContainer;
            Panel.SetZIndex(UserPlaylists, -9999);
        }

        //Change this for JS
        private void timer_Tick(object sender, EventArgs e)
        {
#if OFFINE_IMPLEMENTED //Thanks for the baseplate 
//http://www.wpf-tutorial.com/audio-video/how-to-creating-a-complete-audio-video-player/
            if ((mediaElement.Source != null) && (mediaElement.NaturalDuration.HasTimeSpan) && (!sliderDragging))
            {
                PlayBarSlider.Minimum = 0;
                PlayBarSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                PlayBarSlider.Value = mediaElement.Position.TotalSeconds;
            }
#endif
            if (_songDuration > 0 && !_sliderDragging && _songLoaded)
                media.CefPlayer.GetMainFrame().EvaluateScriptAsync(CurrentTimeScript).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                        Dispatcher.Invoke(() =>
                        {
                            PlayBarSlider.Minimum = 0;
                            PlayBarSlider.Maximum = _songDuration;
                            Console.WriteLine(response.Result.ToString());
                            try
                            {
                                PlayBarSlider.Value = (double)response.Result;
                                if (Math.Abs(PlayBarSlider.Value - PlayBarSlider.Maximum) < 0.001)
                                    PlayBackEnded();
                            }
                            catch
                            {
                                Console.WriteLine("Media hasn't loaded yet");
                            }
                        });
                });
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
#if USING_SPECIAL_EFFECTS //not used
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
            QueryYoutube.QueryVideoList(SearchBox.Text);
            //add card add logic here
            for (var i = 0; i < QueryYoutube.SongCount; i++)
            {
                var artistName = QueryYoutube.SearchListResponse.Items[i].Snippet.ChannelTitle;
                if (artistName.Contains("VEVO"))
                    artistName.Replace("VEVO", "");
                _musicCards = new MusicCards(QueryYoutube.SearchListResponse.Items[i].Id.VideoId,
                    QueryYoutube.SearchListResponse.Items[i].Snippet.Title, artistName,
                    new Uri(QueryYoutube.SearchListResponse.Items[i].Snippet.Thumbnails.Medium.Url),
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
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void Play_Pause_Button_Click(object sender, EventArgs e)
        {

            if (IsPlaying)
            {
                //mediaElement.Pause();
                //mediaElement.Opacity = 0;
                //media.CefPlayer.ShowDevTools();
                var script =
                    "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.pause();})();";
                media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                IsPlaying = false;
                Uri playUri = new Uri("Icons/Play.png", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(playUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                Play_Pause_Button.Background = new ImageBrush(temp);
            }
            else if (_songLoaded)
            {
                var script =
                    "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.play();})();";
                media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                //mediaElement.Play();
                ///mediaElement.Opacity = 100;
                IsPlaying = true;
                Uri pauseUri = new Uri("Icons/Stop.png", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(pauseUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                Play_Pause_Button.Background = new ImageBrush(temp);
            }
        }

        private void PanelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (IsPlaying)
                {
                    //mediaElement.Pause();
                    var script =
                        "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.pause();})();";
                    media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                    IsPlaying = false;
                }
                else
                {
                    //mediaElement.Play();
                    var script =
                        "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.play();})();";
                    media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                    IsPlaying = true;
                }
            }

        }

        private void PlayBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            PlayBarSlider = (Slider)sender;
            _sliderDragging = true;
        }

        private void PlayBar_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            PlayBarSlider = (Slider)sender;
            _sliderDragging = false;
            var script =
                string.Format(
                    "(function(){{var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.currentTime = {0}}})();",
                    PlayBarSlider.Value);
            media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
        }

        private void PlayBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayBarSlider = (Slider)sender;
            Dispatcher.Invoke(() =>
            {
                if (PlayBarSlider.Value <= 60 * 60)
                    CurrentTimeLabel.Content = TimeSpan.FromSeconds(PlayBarSlider.Value).ToString(@"mm\:ss");
                else
                    CurrentTimeLabel.Content = TimeSpan.FromSeconds(PlayBarSlider.Value).ToString(@"hh\:mm\:ss");
            });
        }
#if OFFLINE_IMPLEMENTED
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            EndTimeLabel.Content =
TimeSpan.FromSeconds(mediaElement.NaturalDuration.TimeSpan.TotalSeconds).ToString(@"mm\:ss");
            songURI = mediaElement.Source.AbsoluteUri;
        }
#endif
        private void CefPlayer_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            media.CefPlayer.GetMainFrame()
                .EvaluateScriptAsync(
                    "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); return youtubePlayer.duration;})();")
                .ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {
                        _songDuration = (double)response.Result;
                        Dispatcher.Invoke(() =>
                        {
                            if (_songDuration <= 60 * 60)
                                EndTimeLabel.Content = TimeSpan.FromSeconds(_songDuration).ToString(@"mm\:ss");
                            else
                            {
                                EndTimeLabel.Content = TimeSpan.FromSeconds(_songDuration).ToString(@"hh\:mm\:ss");
                            }
                            _songLoaded = true;
                            media.Minimize.Visibility = Visibility.Visible;
                        });
                        // Console.WriteLine("Song duration: " + songDuration);


                    }
                });
            media.CefPlayer.GetMainFrame()
                .EvaluateScriptAsync(
                    "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); return youtubePlayer.volume})();")
                .ContinueWith(
                    x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                //Console.WriteLine("Music Volume " + response.Result);
                                VolumeControl.slider.Value = (int)response.Result;
                                VolumeControl.label.Content = (int)(VolumeControl.slider.Value * 100) + "%";
                            });
                        }
                    });
        }

        private async void GoLeftButtonClick(object sender, EventArgs e)
        {
            SetIndex(_index - 1);
            //make it play the last song
            if (GetIndex() < 0)
            {
                SetIndex(QueryYoutube.SearchListResponse.Items.Count - 1);
                await
                    GetSong.PlaySpecifiedSong(Background,
                        QueryYoutube.SearchListResponse.Items[QueryYoutube.SearchListResponse.Items.Count - 1].Id.VideoId,
                        _index,
                        QueryYoutube.SearchListResponse.Items[QueryYoutube.SearchListResponse.Items.Count - 1].Snippet
                            .Title,
                        CurrentSong, media.CefPlayer);
            }
            else
            {
                //otherwise play the next song
                await GetSong.PlaySpecifiedSong(Background,
                    QueryYoutube.SearchListResponse.Items[_index].Id.VideoId,
                    _index,
                    QueryYoutube.SearchListResponse.Items[_index].Snippet.Title,
                    CurrentSong, media.CefPlayer);
            }
        }

        private async void PlayBackEnded()
        {
            IsPlaying = false;
            _songLoaded = false;
            if (_isReplay)
            {
                media.CefPlayer.GetMainFrame()
                    .ExecuteJavaScriptAsync(
                        "var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.currentTime = 0; youtubePlayer.play();");
                IsPlaying = true;
                _songLoaded = true;
            }
            else if (_isShuffle)
            {
                var random = new Random();
                await GetSong.AutoPlaySong(random.Next(0, 50), CurrentSong, MusicContainer, Background, media.CefPlayer);
                IsPlaying = true;
                _songLoaded = true;
            }
            else
            {
                await GetSong.AutoPlaySong(_index + 1, CurrentSong, MusicContainer, Background, media.CefPlayer);
                IsPlaying = true;
                _songLoaded = true;
            }
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

        private void Replay_Button_Click(object sender, EventArgs e)
        {
            if (_isReplay)
                _isReplay = false;
            else
                _isReplay = true;
        }

        private void VolumeControl_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            VolumeControl.Volume = VolumeControl.slider.Value;
            int labelContent = (int)(VolumeControl.Volume * 100);
            VolumeControl.label.Content = labelContent + "%";
            //_mediaElement.Volume = Volume;
            media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(String.Format("(function(){{var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.volume = {0}}})();", VolumeControl.Volume));
        }

        private void VolumeControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(VolumeControl, -99999);
        }

        private void Volume_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_songLoaded || IsPlaying)
                Panel.SetZIndex(VolumeControl, 3);
        }

        private void VolumeControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex(VolumeControl, -99999);
        }

        private void MusicPanel_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                media.CefPlayer.GetMainFrame()
                    .ExecuteJavaScriptAsync(
                        "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.currentTime = youtubePlayer.currentTime+10;})();");
            }
            else if (e.Key == Key.Left)
            {
                media.CefPlayer.GetMainFrame()
                    .ExecuteJavaScriptAsync(
                        "(function(){var youtubePlayer = document.getElementById('youtubePlayer'); youtubePlayer.currentTime = youtubePlayer.currentTime-10;})();");
            }
        }

        private void Shuffle_Button_OnClick(object sender, EventArgs e)
        {
            if (_isShuffle)
            {
                _isShuffle = false;
            }
            else
            {
                _isShuffle = true;
                _isReplay = false;
            }
        }


        private async void Next_Song_Button_OnClick(object sender, EventArgs e)
        {
            BoredLabel.IsEnabled = false;
            SetIndex(_index + 1);
            //make it play the first song
            if (GetIndex() == MusicContainer.Children.Count)
            {
                // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                SetIndex(0);
                await
                    GetSong.PlaySpecifiedSong(Background,
                        QueryYoutube.SearchListResponse.Items[0].Id.VideoId,
                        _index,
                        QueryYoutube.SearchListResponse.Items[0].Snippet.Title,
                        CurrentSong, media.CefPlayer);
            }
            else
            {
                //otherwise play the next song
                await GetSong.PlaySpecifiedSong(Background,
                    QueryYoutube.SearchListResponse.Items[_index].Id.VideoId,
                    _index,
                    QueryYoutube.SearchListResponse.Items[_index].Snippet.Title,
                    CurrentSong, media.CefPlayer);
            }
        }

        private void VolumeControl_LostFocus(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(VolumeControl, -99999);
        }

        private void SearchBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Text = "";
        }
        //#TODO Create method to show dropdown options for videos when typing 
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //TODO: create UI for playlist and implement the UI switching (songs and playlists) here
        private void SongSelector_Click(object sender, RoutedEventArgs e)
        {
            SongSelector.Background = new SolidColorBrush(_selectedColor);
            PlaylistSelector.Background = new SolidColorBrush(_deselectedColor);
            _playingSongs = true;
            Panel.SetZIndex(SongSearchContainer, 3);
            FullGrid.Children.Remove(SongSearchContainer);
            TransitioningContentControl.Content = SongSearchContainer;
            Panel.SetZIndex(UserPlaylists, -9999);
        }

        private async void PlaylistSelector_Click(object sender, RoutedEventArgs e)
        {
            PlaylistSelector.Background = new SolidColorBrush(_selectedColor);
            SongSelector.Background = new SolidColorBrush(_deselectedColor);
            _playingSongs = false;
            if (_firstSwitch)
            {
                Panel.SetZIndex(BoredLabel, -9999);
                _firstSwitch = false;
                CurrentSong.Text = "Loading Your Playlists...";
                //init user playlist
                var userPlaylists = await PredefinedPlaylists.GetUserPlaylist();
                foreach (Playlist userPlaylistResponse in userPlaylists.Items)
                {
                    Console.WriteLine(userPlaylistResponse.Id);
                    var playlistItems = await GetMusic.PlaylistToPlaylistItems(userPlaylistResponse.Id);
                    UserPlaylist_Content.Children.Add(
                        new PlaylistContainer(playlistItems, userPlaylistResponse.Snippet.Title, CurrentSong, Background, media.CefPlayer));
                }
                CurrentSong.Text = "Now Playing: nothing!";
                Console.WriteLine("Created User Playlists");
            }
            Panel.SetZIndex(UserPlaylists, 3);
            FullGrid.Children.Remove(UserPlaylists);
            TransitioningContentControl.Content = UserPlaylists;
             Panel.SetZIndex(SongSearchContainer, -9999);
        }
        private void SongSelector_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_playingSongs)
                SongSelector.Background = new SolidColorBrush(_hoverColor);
        }

        private void SongSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_playingSongs)
                SongSelector.Background = new SolidColorBrush(_deselectedColor);
        }

        private void PlaylistSelector_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_playingSongs)
                PlaylistSelector.Background = new SolidColorBrush(_hoverColor);
        }

        private void PlaylistSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_playingSongs)
                PlaylistSelector.Background = new SolidColorBrush(_deselectedColor);
        }
    }
}