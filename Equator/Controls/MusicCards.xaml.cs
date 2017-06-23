using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Equator.Helpers;
using Equator.Music;
using Path = System.IO.Path;
using CefSharp.Wpf;

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
        private readonly Label _songLabel;
        private readonly ChromiumWebBrowser youtubePlayer;
        private Label _endTimeLabel;
        private Slider _playBarSlider;
        public int index;

        public MusicCards(string musicLink, string songTitle, string artistName, Uri backgroundImageUri,
            ref MediaElement mediaElement, ref Label songLabel, ref Label endTimeLabel,
            ref Rectangle backgroundRectangle, ref Slider slider, int index, ref ChromiumWebBrowser youtubePlayer)
        {
            //Set the text to song name 
            //set image to song thumb
            InitializeComponent();
            SongTitle.Text = songTitle;
            Artist_name.Content = artistName;
            _musicLink = musicLink;

            var backgroundBrush = new ImageBrush(new BitmapImage(backgroundImageUri));
            backgroundBrush.Stretch = Stretch.UniformToFill;
            MusicImage.Fill = backgroundBrush;

            MusicCardContent.MouseEnter += MusicCard_Content_MouseEnter;
            MusicCardContent.MouseLeave += MusicCard_Content_MouseLeave;
            Overlay.Opacity = 0;
            Play.Opacity = 0;
            _mediaElement = mediaElement;
            _songLabel = songLabel;
            _endTimeLabel = endTimeLabel;
            _playBarSlider = slider;
            this.index = index;
            _backgroundRect = backgroundRectangle;
            this.youtubePlayer = youtubePlayer;
        }

        private void MusicCard_Content_MouseLeave(object sender, MouseEventArgs e)
        {
            MusicCardContent = (Canvas) sender;
            Play.Opacity = 0;
            Overlay.Opacity = 0;
        }

        public void MusicCard_Content_MouseEnter(object sender, MouseEventArgs e)
        {
            MusicCardContent = (Canvas) sender;
            Play.Opacity = 100;
            Overlay.Opacity = 0.4;
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
            await GetSong.PlaySpecifiedSong(_backgroundRect, _mediaElement, _musicLink, index, SongTitle.Text, _songLabel, youtubePlayer);

        }
}}