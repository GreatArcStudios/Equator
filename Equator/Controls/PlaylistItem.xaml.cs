using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using Equator.Music;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for PlaylistItem.xaml
    /// </summary>
    public partial class PlaylistItem : UserControl
    {
        private string _musicLink;
        private ChromiumWebBrowser _youtubePlayer;
        private Rectangle _backgroundRect;
        private TextBlock _songLabel;
        public PlaylistItem(Uri backgroundImageUri, TextBlock songLabel, string musicLink, string songTitle, string artistName, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer)
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
            await GetSong.PlaySpecifiedSong(_backgroundRect, _musicLink, SongTitle.Text, _songLabel, _youtubePlayer);
        }
    }
}
