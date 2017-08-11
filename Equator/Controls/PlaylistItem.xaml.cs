using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp.Wpf;
using Equator.Helpers;
using Equator.Music;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistItem.xaml
    /// </summary>
    public partial class PlaylistItem : UserControl
    {
        internal int Index {
            get => _index;
            set => _index = value;
        }
        internal string MusicLink
        {
            get => _musicLink;
            set => _musicLink = value;
        }

        internal PlaylistItemListResponse ParentPlaylist => _parentPlaylist;
        internal string BackgroundImageUrl => _backgroundImageUrl;
        private int _index;
        private string _musicLink;
        private readonly string _backgroundImageUrl;
        private readonly Rectangle _backgroundRect;
        private readonly PlaylistItemListResponse _parentPlaylist;
        private readonly TextBlock _songLabel;
        private readonly ChromiumWebBrowser _youtubePlayer;

        public PlaylistItem(Uri backgroundImageUri, TextBlock songLabel, string musicLink, string songTitle,
            string artistName, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer, int index,
            PlaylistItemListResponse playlistItemListResponse)
        {
            InitializeComponent();
            var backgroundBrush = new ImageBrush(new BitmapImage(backgroundImageUri));
            backgroundBrush.Stretch = Stretch.UniformToFill;
            MusicImage.Fill = backgroundBrush;
            SongTitle.Text = songTitle;
            ArtistName.Text = artistName;
            _musicLink = musicLink;
            Overlay.Opacity = 0;
            Play.Opacity = 0;
            _youtubePlayer = youtubePlayer;
            _backgroundRect = backgroundRectangle;
            _songLabel = songLabel;
            _backgroundImageUrl = backgroundImageUri.ToString();
            _index = index;
            _parentPlaylist = playlistItemListResponse;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Play.Opacity = 100;
            Overlay.Opacity = 0.4;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Overlay.Opacity = 0;
            Play.Opacity = 0;
        }

        private async void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Play.Opacity = 100;
            MusicPanel.PlayListIndex = _index;
            MusicPanel.PlayedPlaylistIndicies.Add(_index);
            MusicPanel.PlayingSongs = false;
            Console.WriteLine("Playing Songs? " + MusicPanel.PlayingSongs);
            QueryYoutube.CurrentPlaylistItemListResponse = _parentPlaylist;
            await Music.Music.PlaySpecifiedSong(_backgroundRect, _musicLink, SongTitle.Text, _songLabel, _youtubePlayer,
                _backgroundImageUrl);
        }
    }
}