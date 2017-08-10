using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp.Wpf;
using Equator.Music;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for Music_Cards.xaml
    /// </summary>
    public partial class MusicCards : UserControl
    {
        private readonly Rectangle _backgroundRect;
        private readonly MediaElement _mediaElement;
        private readonly string _musicLink;
        private readonly TextBlock _songLabel;
        private readonly ChromiumWebBrowser _youtubePlayer;
        private Label _endTimeLabel;
        private Slider _playBarSlider;
        public int Index;

        public MusicCards(string musicLink, string songTitle, string artistName, Uri backgroundImageUri,
            ref MediaElement mediaElement, ref TextBlock songLabel, ref Label endTimeLabel,
            ref Rectangle backgroundRectangle, ref Slider slider, int index, ref ChromiumWebBrowser youtubePlayer)
        {
            //Set the text to song name 
            //set image to song thumb
            InitializeComponent();
            SongTitle.Text = songTitle;
            ArtistName.Content = artistName;
            _musicLink = musicLink;

            var backgroundBrush = new ImageBrush(new BitmapImage(backgroundImageUri));
            backgroundBrush.Stretch = Stretch.UniformToFill;
            MusicImage.Fill = backgroundBrush;

            Overlay.Opacity = 0;
            Play.Opacity = 0;
            _mediaElement = mediaElement;
            _songLabel = songLabel;
            _endTimeLabel = endTimeLabel;
            _playBarSlider = slider;
            Index = index;
            _backgroundRect = backgroundRectangle;
            _youtubePlayer = youtubePlayer;
        }

        public MusicCards(string musicLink, string songTitle, string artistName, Uri backgroundImageUri,
            ref TextBlock songLabel, ref Label endTimeLabel,
            ref Rectangle backgroundRectangle, ref Slider slider, int index, ref ChromiumWebBrowser youtubePlayer)
        {
            InitializeComponent();
            SongTitle.Text = songTitle;
            ArtistName.Content = artistName;
            _musicLink = musicLink;

            var backgroundBrush = new ImageBrush(new BitmapImage(backgroundImageUri));
            backgroundBrush.Stretch = Stretch.UniformToFill;
            MusicImage.Fill = backgroundBrush;

            Overlay.Opacity = 0;
            Play.Opacity = 0;
            _songLabel = songLabel;
            _endTimeLabel = endTimeLabel;
            _playBarSlider = slider;
            Index = index;
            _backgroundRect = backgroundRectangle;
            _youtubePlayer = youtubePlayer;
        }


        /// <summary>
        ///     Returns a string split into the format "link to the music/video" + "song name"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async void LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            MusicCardContent = (Canvas) sender;
            Play.Opacity = 100;
            MusicPanel.PlayingSongs = true;
            await Music.Music.PlaySpecifiedSong(_backgroundRect, _musicLink, Index, SongTitle.Text, _songLabel,
                _youtubePlayer);
        }

        private void MusicCardContent_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard) FindResource("Fadeinplay")).Begin(Play);
            ((Storyboard) FindResource("Fadeinoverlay")).Begin(Overlay);
        }

        private void MusicCardContent_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard) FindResource("Fadeoutplay")).Begin(Play);
            ((Storyboard) FindResource("Fadeoutoverlay")).Begin(Overlay);
        }
    }
}