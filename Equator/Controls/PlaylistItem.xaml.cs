using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp.Wpf;
using Equator.Music;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistItem.xaml
    /// </summary>
    public partial class PlaylistItem : UserControl
    {
        private readonly string _backgroundImageUrl;
        private readonly Rectangle _backgroundRect;
        private readonly int _index;
        private readonly string _musicLink;
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

        private void MusicImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Play.Opacity = 100;
            Overlay.Opacity = 0.4;
        }

        private void MusicImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Overlay.Opacity = 0;
            Play.Opacity = 0;
        }

        private async void MusicImage_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)
        {
            Play.Opacity = 100;
            MusicPanel.PlayListIndex = _index;
            Playlists.CurrentPlaylistItemListResponse = _parentPlaylist;
            await GetSong.PlaySpecifiedSong(_backgroundRect, _musicLink, SongTitle.Text, _songLabel, _youtubePlayer,
                _backgroundImageUrl);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
        }
    }
}