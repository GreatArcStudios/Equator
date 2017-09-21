using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CefSharp;
using Equator.Controls;
using Equator.Helpers;
using Equator.Music;
using MahApps.Metro.Controls;
using Color = System.Windows.Media.Color;
using Image = System.Drawing.Image;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MusicPanel.xaml
    /// </summary>
    public partial class MusicPanel : MetroWindow
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static bool IsPlaying;
        public static int PlayListIndex
        {
            get { return _playListIndex; }
            set { _playListIndex = value; }
        }

        public static bool PlayingSongs
        {
            get => _playingSongs;
            set => _playingSongs = value;
        }

        public static int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public static int ReplayState
        {
            get { return _replayState; }
            set { _replayState = value; }
        }

        public enum ReplayStates
        {
            Off, ReplayAll, ReplaySong
        }

        public static List<int> PlayedPlaylistIndiciesBackwards = new List<int>();
        public static List<int> PlayedPlaylistIndiciesFowards = new List<int>();
        public static List<int> PlayedIndiciesBackwards = new List<int>();
        private List<int> PlayedIndiciesForward = new List<int>();
        private static int _playListIndex;
        private static int _index;
        private MusicCards _musicCards;
        private bool _sliderDragging;
        private static int _replayState;
        private bool _isShuffle;
        private static bool _playingSongs = true;
        private static bool _searchingSongs = true;
        private readonly Color _selectedColor = Color.FromArgb(127, 180, 180, 180);
        private readonly Color _deselectedColor = Color.FromArgb(127, 126, 126, 126);
        private readonly Color _hoverColor = Color.FromArgb(127, 170, 170, 170);
        private Color _pressedColor = Color.FromArgb(153, 60, 60, 60);

        //private string songURI;
        private const string CurrentTimeScript =
                "(function(){return youtubePlayer.getCurrentTime();})();"
            ;
        private const string CheckPlaybackEndedScript =
                "(function(){return playBackEnded;})();"
            ;
        private double _songDuration;
        private bool _songLoaded;
        private bool _firstSwitch = true;
        private static bool _creatingSongCards;
        private static bool _creatingPlaylistCards;



        public MusicPanel()
        {
#if DEBUG
           GoogleServices.AuthUserCredential(true);
           GoogleServices.YoutubeService =
                GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, true, GoogleServices.Credential);
#endif
            InitializeComponent();
            //create user image
            ImageBrush userImageBrush;
            if (!File.Exists(FilePaths.UserImageLocation + "\\Userimage.png"))
                userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
            else
                userImageBrush =
                    new ImageBrush(new BitmapImage(new Uri(FilePaths.UserImageLocation + "\\Userimage.png")));
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

                var image = Image.FromFile(new Uri(@"Images\Grass Blades.jpg", UriKind.Relative).ToString());
                var blur = new GaussianBlur(image as Bitmap);
                var blurredThumb = blur.Process(20);
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
            Media.CefPlayer.LoadingStateChanged += CefPlayer_LoadingStateChanged;
            //init trans content control
            //FullGrid.Children.Remove(SongSearchContainer);
            //TransitioningContentControl.Content = SongSearchContainer;
            Panel.SetZIndex(Media, -9999);

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
            {
                Media.CefPlayer.GetMainFrame().EvaluateScriptAsync(CurrentTimeScript).ContinueWith(x =>
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
                           }
                           catch
                           {
                               Console.WriteLine("Media hasn't loaded yet");
                           }
                       });
               });
                Media.CefPlayer.GetMainFrame().EvaluateScriptAsync(CheckPlaybackEndedScript).ContinueWith(x =>
                {
                    var response = x.Result;
                    Console.WriteLine(response.Result);
                    if (response.Success && response.Result != null)
                    {

                        if (response.Result.ToString().ToLower().Equals("true"))
                            Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        PlayBackEnded();
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Media hasn't loaded yet");
                                    }

                                }
                                   );
                    }
                });
            }

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

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(BoredLabel, -9999);
            if (_searchingSongs)
            {
                if (!_creatingSongCards)
                {
                    _creatingSongCards = true;
                    _index = -1;
                    MusicContainer.Children.RemoveRange(0, MusicContainer.Children.Count);
                    await QueryYoutube.QueryVideoListAsync(SearchBox.Text);
                    //add card add logic here
                    if (QueryYoutube.SongSearchListResponse.Items.Count > 0)
                        for (var i = 0; i < QueryYoutube.SongSearchListResponse.Items.Count; i++)
                        {
                            var artistName = QueryYoutube.SongSearchListResponse.Items[i].Snippet.ChannelTitle;
                            var songTitle = QueryYoutube.SongSearchListResponse.Items[i].Snippet.Title;
                            if (!artistName.ToLower().Contains("topic") && !artistName.ToLower().Equals(songTitle) || i == 0)
                            {
                                _musicCards = new MusicCards(QueryYoutube.SongSearchListResponse.Items[i].Id.VideoId,
                                    QueryYoutube.SongSearchListResponse.Items[i].Snippet.Title, artistName,
                                    new Uri(QueryYoutube.SongSearchListResponse.Items[i].Snippet.Thumbnails.Medium.Url),
                                    ref CurrentSong, ref EndTimeLabel, ref Background, ref PlayBarSlider, i,
                                    ref Media.CefPlayer, ref PlayPauseButton);
                                MusicContainer.Children.Add(_musicCards);
                            }
                        }
                    else
                        CurrentSong.Text = "No Songs Found!";
                    _creatingSongCards = false;
                }

                GC.Collect();
            }
            else
            {
                if (!_creatingPlaylistCards)
                {
                    _creatingPlaylistCards = true;
                    PlaylistsHolder.Children.Clear();
                    ExpandedPlaylistHolder.Children.Clear();
                    this.UpdateLayout();

                    await QueryYoutube.QueryPlaylistListAsync(SearchBox.Text);
                    var oldText = CurrentSong.Text;
                    CurrentSong.Text = "Loading Playlists...";
                    var userPlaylists = await QueryYoutube.QueryUserPlaylistsAsync();
                    PlaylistsHolder.Children.Add(new PlaylistCards(true, false, "null", userPlaylists
                        , null, CurrentSong, Background, Media.CefPlayer, ExpandedPlaylistHolder, null, PlaylistScrollView, PlayPauseButton));
                    if (QueryYoutube.PlaylistSearchListResponse.Items.Count > 0)
                    {
                        for (var i = 0; i < QueryYoutube.PlaylistSearchListResponse.Items.Count; i++)
                        {
                            var playlistId = QueryYoutube.PlaylistSearchListResponse.Items[i].Id.PlaylistId;
                            if (playlistId != null)
                            {
                                var playlistListResponse = await QueryYoutube.PlaylistToPlaylistItems(playlistId);
                                PlaylistsHolder.Children.Add(new PlaylistCards(false, false, QueryYoutube.PlaylistSearchListResponse.Items[i].Snippet.Title, null
                                    , playlistListResponse, CurrentSong, Background, Media.CefPlayer, ExpandedPlaylistHolder, null, PlaylistScrollView, PlayPauseButton));
                            }
                        }

                        if (oldText.Equals("Now Playing: nothing!"))
                            CurrentSong.Text = "Now Playing: nothing!";
                        else
                            CurrentSong.Text = oldText;
                    }
                    else
                        CurrentSong.Text = "No Songs Found!";
                    _creatingPlaylistCards = false;
                }

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
                    "(function(){youtubePlayer.pauseVideo();})();";
                Media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                IsPlaying = false;
                var playUri = new Uri("Icons/Play.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(playUri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                PlayPauseButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
            else if (_songLoaded)
            {
                var script =
                    "(function(){youtubePlayer.playVideo();})();";
                Media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                //mediaElement.Play();
                ///mediaElement.Opacity = 100;
                IsPlaying = true;
                var pauseUri = new Uri("Icons/Stop.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(pauseUri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                PlayPauseButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
        }

        private void PanelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                if (IsPlaying)
                {
                    //mediaElement.Pause();
                    var script =
                        "(function(){youtubePlayer.pauseVideo();})();";
                    Media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                    IsPlaying = false;
                    var playUri = new Uri("Icons/Play.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(playUri);
                    var temp = BitmapFrame.Create(streamInfo.Stream);
                    PlayPauseButton.Background = new ImageBrush(temp);
                }
                else
                {
                    //mediaElement.Play();
                    var script =
                        "(function(){youtubePlayer.playVideo();})();";
                    Media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
                    IsPlaying = true;
                    var playUri = new Uri("Icons/Stop.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(playUri);
                    var temp = BitmapFrame.Create(streamInfo.Stream);
                    PlayPauseButton.Background = new ImageBrush(temp);
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
                $"(function(){{youtubePlayer.seekTo({PlayBarSlider.Value});}})();";
            Media.CefPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
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
            Media.CefPlayer.GetMainFrame()
                .EvaluateScriptAsync(
                    "(function(){return youtubePlayer.getDuration();})();")
                .ContinueWith(x =>
                {
                    var response = x.Result;
                    Console.WriteLine(response.Result);
                    if (response.Success && response.Result != null)
                    {
                        try
                        {
                            _songDuration = (double)response.Result;
                        }
                        catch
                        {
                            _songDuration = (int)response.Result;
                        }

                        Dispatcher.Invoke(() =>
                        {
                            if (_songDuration <= 60 * 60)
                            {
                                EndTimeLabel.Content = TimeSpan.FromSeconds(_songDuration).ToString(@"mm\:ss");
                                // CurrentTimeLabel.Content = TimeSpan.FromSeconds(_songDuration).ToString(@"mm\:ss");
                                PlayBarSlider.Width = 895;
                            }
                            else
                            {
                                EndTimeLabel.Content = TimeSpan.FromSeconds(_songDuration).ToString(@"hh\:mm\:ss");
                                // CurrentTimeLabel.Content = TimeSpan.FromSeconds(_songDuration).ToString(@"hh\:mm\:ss");
                                PlayBarSlider.Width = 870;
                            }
                            _songLoaded = true;
                            Panel.SetZIndex(Media, 3);
                        });
                        // Console.WriteLine("Song duration: " + songDuration);
                    }
                });
            Media.CefPlayer.GetMainFrame()
                .EvaluateScriptAsync(
                    "(function(){return youtubePlayer.getVolume();})();")
                .ContinueWith(
                    x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                            Dispatcher.Invoke(() =>
                            {
                                //Console.WriteLine("Music Volume " + response.Result);
                                VolumeControl.Slider.Value = (int)response.Result;
                                VolumeControl.Label.Content = (int)(VolumeControl.Slider.Value * 100) + "%";
                            });
                    });
        }



        private async void PlayBackEnded()
        {
            IsPlaying = false;
            _songLoaded = false;
            if (ReplayState != (int)ReplayStates.ReplaySong && !_isShuffle)
            {
                if (_playingSongs)
                {
                    if (Index == Music.QueryYoutube.SongSearchListResponse.Items.Count - 1)
                    {
                        if (ReplayState == (int)ReplayStates.Off)
                        {
                            return;
                        }
                        else
                        {
                            Index = 0;
                        }

                    }
                    else
                    {
                        Index++;
                    }

                    if (!await Music.Music.AutoPlaySong(Index, CurrentSong, Background, Media.CefPlayer,
                        false, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                    IsPlaying = true;
                    _songLoaded = true;
                }
                else
                {
                    if (_playListIndex == Music.QueryYoutube.CurrentPlaylistItemListResponse.Items.Count - 1)
                    {
                        if (ReplayState == (int)ReplayStates.Off)
                        {
                            return;
                        }
                        else
                        {
                            _playListIndex = 0;
                        }

                    }
                    else
                    {
                        _playListIndex++;
                    }
                    if (!await Music.Music.AutoPlaySong(_playListIndex, CurrentSong, Background,
                        Media.CefPlayer,
                        true, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                    PlayedPlaylistIndiciesBackwards.Add(_playListIndex);
                    IsPlaying = true;
                    _songLoaded = true;
                }
            }
            else if (ReplayState == (int)ReplayStates.ReplaySong)
            {
                Media.CefPlayer.GetMainFrame()
                    .ExecuteJavaScriptAsync(
                        "youtubePlayer.seekTo(0, true); youtubePlayer.playVideo();");
                IsPlaying = true;
                _songLoaded = true;
            }
            else if (_isShuffle)
            {
                if (_playingSongs)
                {
                    var random = new Random();
                    int generatedIndex = random.Next(0, QueryYoutube.SongSearchListResponse.Items.Count);
                    _index = generatedIndex;
                    await Music.Music.AutoPlaySong(generatedIndex, CurrentSong, Background,
                        Media.CefPlayer, false, PlayPauseButton);
                    IsPlaying = true;
                    _songLoaded = true;
                }
                else
                {
                    var random = new Random();
                    int generatedIndex = random.Next(0, QueryYoutube.CurrentPlaylistItemListResponse.Items.Count);
                    PlayedPlaylistIndiciesBackwards.Add(generatedIndex);
                    _playListIndex = generatedIndex;
                    if (!await Music.Music.AutoPlaySong(generatedIndex, CurrentSong, Background,
                        Media.CefPlayer, true, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                    IsPlaying = true;
                    _songLoaded = true;
                }
            }
        }
        /*
        private async void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            mediaElement.Opacity = 0;
            if (!isReplay)
                await Music.AutoPlaySong(_index + 1, CurrentSong, MusicContainer, mediaElement, Background, );
            else
                mediaElement.Source = new Uri(songURI);
            mediaElement.Play();
        }*/

        private void Replay_Button_Click(object sender, EventArgs e)
        {
            if (ReplayState == (int)ReplayStates.Off)
            {
                ReplayState = (int)ReplayStates.ReplayAll;
                var uri = new Uri("Icons/reply all.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(uri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                ReplayButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
            else if (ReplayState == (int)ReplayStates.ReplayAll)
            {
                ReplayState = (int)ReplayStates.ReplaySong;
                var uri = new Uri("Icons/replay song.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(uri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                ReplayButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
            else if (ReplayState == (int)ReplayStates.ReplaySong)
            {
                ReplayState = (int)ReplayStates.Off;
                var uri = new Uri("Icons/replay off.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(uri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                ReplayButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
        }

        private void VolumeControl_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            VolumeControl.Volume = VolumeControl.Slider.Value * 100;
            var labelContent = (int)(VolumeControl.Volume);
            VolumeControl.Label.Content = labelContent + "%";
            //_mediaElement.Volume = Volume;
            Media.CefPlayer.GetMainFrame()
                .ExecuteJavaScriptAsync(
                    $"(function(){{youtubePlayer.setVolume({VolumeControl.Volume});}})();");
        }

        private void VolumeControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(VolumeControl, -99999);
        }

        private void Volume_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_songLoaded || IsPlaying)
            {
                Panel.SetZIndex(VolumeControl, 3);
                var uri = new Uri("Icons/16 volume selected.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(uri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                VolumeButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
        }

        private void VolumeControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex(VolumeControl, -99999);
            var uri = new Uri("Icons/16 Volume.png", UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            var temp = BitmapFrame.Create(streamInfo.Stream);
            VolumeButton.Background = new ImageBrush(temp);
            streamInfo.Stream.Close();
            streamInfo.Stream.Dispose();
        }

        private void MusicPanel_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
                Media.CefPlayer.GetMainFrame()
                    .ExecuteJavaScriptAsync(
                        "(function(){youtubePlayer.seekTo(youtubePlayer.getCurrentTime()+10, true);})();");
            else if (e.Key == Key.Left)
                Media.CefPlayer.GetMainFrame()
                    .ExecuteJavaScriptAsync(
                        "(function(){youtubePlayer.seekTo(youtubePlayer.getCurrentTime()-10, true);})();");
        }

        private void Shuffle_Button_OnClick(object sender, EventArgs e)
        {
            if (_isShuffle)
            {
                _isShuffle = false;
                var uri = new Uri("Icons/32 shuffle.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(uri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                ShuffleButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
            else
            {
                _isShuffle = true;
                var uri = new Uri("Icons/32 shuffle selected.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(uri);
                var temp = BitmapFrame.Create(streamInfo.Stream);
                ShuffleButton.Background = new ImageBrush(temp);
                streamInfo.Stream.Close();
                streamInfo.Stream.Dispose();
            }
        }
        private async void Last_Song_Button_OnClick(object sender, EventArgs e)
        {
            if (_playingSongs)
            {
                if (PlayedIndiciesBackwards.Count - 1 > 0)
                {
                    if (PlayedIndiciesBackwards.Count - 1 != 0)
                    {
                        PlayedIndiciesForward.Add(PlayedIndiciesBackwards[PlayedIndiciesBackwards.Count - 1]);
                        PlayedIndiciesBackwards.RemoveAt(PlayedIndiciesBackwards.Count - 1);
                        _index = PlayedIndiciesBackwards.Last();
                    }
                    else
                    {
                        PlayedIndiciesForward.Add(PlayedIndiciesBackwards[PlayedIndiciesBackwards.Count - 1]);
                        _index = PlayedIndiciesBackwards.Last();
                        PlayedIndiciesBackwards.RemoveAt(PlayedIndiciesBackwards.Count - 1);
                    }
                    if (!await
                        Music.Music.PlaySpecifiedSong(Background,
                            QueryYoutube.SongSearchListResponse.Items[Index].Id
                                .VideoId,
                            Index,
                            QueryYoutube.SongSearchListResponse.Items[Index]
                                .Snippet
                                .Title,
                            CurrentSong, Media.CefPlayer, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                }
                else
                {
                    if (Index != 0)
                    {
                        Index--;
                        if (PlayedIndiciesBackwards.Count == 1)
                        {
                            PlayedIndiciesForward.Add(PlayedIndiciesBackwards[PlayedIndiciesBackwards.Count - 1]);
                            PlayedIndiciesBackwards.RemoveAt(PlayedIndiciesBackwards.Count - 1);
                        }
                        if (!await
                            Music.Music.PlaySpecifiedSong(Background,
                                QueryYoutube.SongSearchListResponse.Items[Index].Id
                                    .VideoId,
                                Index,
                                QueryYoutube.SongSearchListResponse.Items[Index]
                                    .Snippet
                                    .Title,
                                CurrentSong, Media.CefPlayer, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                    else
                    {
                        Index = QueryYoutube.SongSearchListResponse.Items.Count - 1;
                        if (PlayedIndiciesBackwards.Count == 1)
                        {
                            PlayedIndiciesForward.Add(PlayedIndiciesBackwards[PlayedIndiciesBackwards.Count - 1]);
                            PlayedIndiciesBackwards.RemoveAt(PlayedIndiciesBackwards.Count - 1);
                        }
                        if (!await
                            Music.Music.PlaySpecifiedSong(Background,
                                QueryYoutube.SongSearchListResponse.Items[Index].Id
                                    .VideoId,
                                Index,
                                QueryYoutube.SongSearchListResponse.Items[Index]
                                    .Snippet
                                    .Title,
                                CurrentSong, Media.CefPlayer, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }

                }

            }
            else
            {

                if (PlayedPlaylistIndiciesBackwards.Count - 1 > 0)
                {
                    if (PlayedPlaylistIndiciesBackwards.Count - 1 != 0)
                    {
                        PlayedPlaylistIndiciesFowards.Add(PlayedPlaylistIndiciesBackwards[PlayedPlaylistIndiciesBackwards.Count - 1]);
                        PlayedPlaylistIndiciesBackwards.RemoveAt(PlayedPlaylistIndiciesBackwards.Count - 1);
                        _index = PlayedPlaylistIndiciesBackwards.Last();
                    }
                    else
                    {
                        PlayedPlaylistIndiciesFowards.Add(PlayedPlaylistIndiciesBackwards[PlayedPlaylistIndiciesBackwards.Count - 1]);
                        _index = PlayedPlaylistIndiciesBackwards.Last();
                        PlayedPlaylistIndiciesBackwards.RemoveAt(PlayedPlaylistIndiciesBackwards.Count - 1);
                    }
                    _playListIndex = PlayedPlaylistIndiciesBackwards.Last();
                    if (!await Music.Music.PlaySpecifiedSong(Background,
                        QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.ResourceId.VideoId,
                        QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Title, CurrentSong,
                        Media.CefPlayer,
                        QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Thumbnails.Medium
                            .Url, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                }
                else
                {
                    if (_playListIndex != 0)
                    {
                        _playListIndex--;
                        if (PlayedPlaylistIndiciesBackwards.Count == 1)
                        {
                            PlayedPlaylistIndiciesFowards.Add(PlayedPlaylistIndiciesBackwards[PlayedPlaylistIndiciesBackwards.Count - 1]);
                            PlayedPlaylistIndiciesBackwards.RemoveAt(PlayedPlaylistIndiciesBackwards.Count - 1);
                        }
                        if (!await
                            Music.Music.PlaySpecifiedSong(Background,
                                QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.ResourceId
                                    .VideoId,
                                QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Title,
                                CurrentSong,
                                Media.CefPlayer,
                                QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Thumbnails
                                    .Medium
                                    .Url, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                    else
                    {
                        _playListIndex = QueryYoutube.CurrentPlaylistItemListResponse.Items.Count - 1;
                        if (PlayedPlaylistIndiciesBackwards.Count == 1)
                        {
                            PlayedPlaylistIndiciesFowards.Add(PlayedPlaylistIndiciesBackwards[PlayedPlaylistIndiciesBackwards.Count - 1]);
                            PlayedPlaylistIndiciesBackwards.RemoveAt(PlayedPlaylistIndiciesBackwards.Count - 1);
                        }
                        if (!await
                            Music.Music.PlaySpecifiedSong(Background,
                                QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.ResourceId
                                    .VideoId,
                                QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Title,
                                CurrentSong,
                                Media.CefPlayer,
                                QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Thumbnails
                                    .Medium
                                    .Url, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                }
            }
        }

        private async void Next_Song_Button_OnClick(object sender, EventArgs e)
        {
            BoredLabel.IsEnabled = false;
            if (_playingSongs)
            {
                if (PlayedIndiciesForward.Count == 0)
                {
                    _index = _index + 1;

                    //make it play the first song
                    if (_index == MusicContainer.Children.Count)
                    {
                        // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                        _index = 0;
                        PlayedIndiciesBackwards.Add(0);
                        if (!await
                            Music.Music.PlaySpecifiedSong(Background,
                                QueryYoutube.SongSearchListResponse.Items[0].Id.VideoId,
                                Index,
                                QueryYoutube.SongSearchListResponse.Items[0].Snippet.Title,
                                CurrentSong, Media.CefPlayer, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                    else
                    {
                        PlayedIndiciesBackwards.Add(Index);
                        //otherwise play the next song
                        if (!await Music.Music.PlaySpecifiedSong(Background,
                            QueryYoutube.SongSearchListResponse.Items[Index].Id.VideoId,
                            Index,
                            QueryYoutube.SongSearchListResponse.Items[Index].Snippet.Title,
                            CurrentSong, Media.CefPlayer, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                }
                else
                {

                    Index = PlayedIndiciesForward.Last();
                    PlayedIndiciesForward.RemoveAt(PlayedIndiciesForward.Count - 1);
                    PlayedIndiciesBackwards.Add(Index);
                    if (!await Music.Music.PlaySpecifiedSong(Background,
                        QueryYoutube.SongSearchListResponse.Items[Index].Id.VideoId,
                        Index,
                        QueryYoutube.SongSearchListResponse.Items[Index].Snippet.Title,
                        CurrentSong, Media.CefPlayer, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                }

            }
            else
            {
                if (PlayedPlaylistIndiciesFowards.Count == 0)
                {
                    _playListIndex++;
                    if (_playListIndex == QueryYoutube.CurrentPlaylistItemListResponse.Items.Count)
                    {
                        _playListIndex = 0;
                        PlayedPlaylistIndiciesBackwards.Add(_playListIndex);
                        if (!await Music.Music.PlaySpecifiedSong(Background,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.ResourceId
                                .VideoId,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Title,
                            CurrentSong,
                            Media.CefPlayer,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Thumbnails.Medium
                                .Url, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                    else
                    {
                        PlayedPlaylistIndiciesBackwards.Add(_playListIndex);
                        //otherwise play the next song
                        if (!await Music.Music.PlaySpecifiedSong(Background,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.ResourceId
                                .VideoId,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Title,
                            CurrentSong,
                            Media.CefPlayer,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Thumbnails.Medium
                                .Url, PlayPauseButton))
                        {
                            MessageBox.Show("An error occured. Please click on a song.");
                        }
                    }
                }
                else
                {
                    _playListIndex = PlayedPlaylistIndiciesFowards.Last();
                    PlayedPlaylistIndiciesFowards.RemoveAt(PlayedPlaylistIndiciesFowards.Count - 1);
                    PlayedPlaylistIndiciesBackwards.Add(_playListIndex);
                    if (!await Music.Music.PlaySpecifiedSong(Background,
                        QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.ResourceId
                            .VideoId,
                        QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Title,
                        CurrentSong,
                        Media.CefPlayer,
                        QueryYoutube.CurrentPlaylistItemListResponse.Items[_playListIndex].Snippet.Thumbnails.Medium
                            .Url, PlayPauseButton))
                    {
                        MessageBox.Show("An error occured. Please click on a song.");
                    }
                }
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
            Panel.SetZIndex(SongSearchContainer, 3);
            FullGrid.Children.Remove(SongSearchContainer);
            TransitioningContentControl.Content = SongSearchContainer;
            Panel.SetZIndex(AllPlaylistParts, -9999);
            _searchingSongs = true;
        }

        private async void PlaylistSelector_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSwitch)
            {
                Panel.SetZIndex(BoredLabel, -9999);
                _firstSwitch = false;
                var oldText = CurrentSong.Text;
                CurrentSong.Text = "Loading Your Playlists...";
                //init user playlist
                var userPlaylists = await QueryYoutube.QueryUserPlaylistsAsync();
                /*foreach (Playlist userPlaylistResponse in userQueryYoutube.Items)
                {
                    Console.WriteLine(userPlaylistResponse.Id);
                    var playlistItems = await QueryYoutube.PlaylistToPlaylistItems(userPlaylistResponse.Id);
                    UserPlaylist_Content.Children.Add(
                        new PlaylistContainer(playlistItems, userPlaylistResponse.Snippet.Title, CurrentSong, Background, media.CefPlayer));

                }*/
                PlaylistsHolder.Children.Add(new PlaylistCards(true, false, "null", userPlaylists
                    , null, CurrentSong, Background, Media.CefPlayer, ExpandedPlaylistHolder, null, PlaylistScrollView, PlayPauseButton));
                if (oldText.Equals("Now Playing: nothing!"))
                    CurrentSong.Text = "Now Playing: nothing!";
                else
                    CurrentSong.Text = oldText;
                Console.WriteLine("Created User Playlists");
            }
            //Panel.SetZIndex(UserPlaylists, 3);
            Panel.SetZIndex(AllPlaylistParts, 3);
            FullGrid.Children.Remove(AllPlaylistParts);
            TransitioningContentControl.Content = AllPlaylistParts;
            Panel.SetZIndex(SongSearchContainer, -9999);
            _searchingSongs = false;
            PlaylistSelector.Background = new SolidColorBrush(_selectedColor);
            SongSelector.Background = new SolidColorBrush(_deselectedColor);
        }

        private void SongSelector_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_searchingSongs)
                SongSelector.Background = new SolidColorBrush(_hoverColor);
        }

        private void SongSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_searchingSongs)
                SongSelector.Background = new SolidColorBrush(_deselectedColor);
        }

        private void PlaylistSelector_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_searchingSongs)
                PlaylistSelector.Background = new SolidColorBrush(_hoverColor);
        }

        private void PlaylistSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_searchingSongs)
                PlaylistSelector.Background = new SolidColorBrush(_deselectedColor);
        }

        private void Minimize_Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (Media.State == (int)YoutubePlayer.WindowStates.Normal)
            {
                ((Storyboard)FindResource("Minimize")).Begin(Media.Container);
                Media.State = (int)YoutubePlayer.WindowStates.Minimized;
            }
            else if (Media.State == (int)YoutubePlayer.WindowStates.Minimized)
            {
                ((Storyboard)FindResource("Normalize")).Begin(Media.Container);
                Media.State = (int)YoutubePlayer.WindowStates.Normal;
            }
            /*
            else if (Media.State == (int) YoutubePlayer.WindowStates.Minimized)
            {
                ((Storyboard) FindResource("Maximize")).Begin(Media);
                Media.State = (int) YoutubePlayer.WindowStates.Maximized;

            }*/
        }

        private void Media_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Media.State == (int)YoutubePlayer.WindowStates.Normal)
            {
                ((Storyboard)FindResource("Minimize")).Begin(Media.Container);
                Media.State = (int)YoutubePlayer.WindowStates.Minimized;
            }
            else if (Media.State == (int)YoutubePlayer.WindowStates.Minimized)
            {
                ((Storyboard)FindResource("Normalize")).Begin(Media.Container);
                Media.State = (int)YoutubePlayer.WindowStates.Normal;
            }
            /*else if (Media.State == (int)YoutubePlayer.WindowStates.Minimized)
           {
               ((Storyboard)FindResource("Maximize")).Begin(Media);
               Media.State = (int)YoutubePlayer.WindowStates.Maximized;
           }*/
        }

        private void MusicPlayer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(FilePaths.ThumbLocation);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
        }
    }
}