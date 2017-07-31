using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for PlaylistCards.xaml
    /// </summary>
    public partial class PlaylistCards : UserControl
    {
        internal PlaylistItemListResponse _playlistItemListResponse;
        internal bool _isUserPlaylist; 
        private string _playlistName;
        private TextBlock _songLabel; 
        private bool _firstShow = true;
        private Rectangle _backgroundRectangle;
        private ChromiumWebBrowser _youtubePlayer;
        private Grid _expandedPlaylistHolder; 
        public PlaylistCards(bool userPlaylist, string playlistName, PlaylistItemListResponse playlistItemListResponse, TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer, Grid expandedPlaylistHolder)
        {
            InitializeComponent();
            Overlay.Opacity = 0;
            _playlistItemListResponse = playlistItemListResponse;
            _isUserPlaylist = userPlaylist;
            Console.WriteLine(playlistName + " has " + playlistItemListResponse.Items.Count);
            if (userPlaylist)
            {
                //create user image
                ImageBrush userImageBrush;
                if (!File.Exists(FilePaths.SaveUserImage() + "\\Userimage.png"))
                    userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
                else
                {
                    userImageBrush =
                        new ImageBrush(new BitmapImage(new Uri(FilePaths.SaveUserImage() + "\\Userimage.png")));
                }
                userImageBrush.TileMode = TileMode.None;
                UserPlaylistCover.Fill = userImageBrush;
                PlaylistName.Text = playlistName; 
                Dispatcher.Invoke(() => { SearchedPlaylistImagesCover.IsEnabled = false; });
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Uri backgroundImageUri = new Uri(_playlistItemListResponse.Items[i].Snippet.Thumbnails.Medium.Url);
                    ImageSource tempSource = new BitmapImage(backgroundImageUri);
                    ((Image)SearchedPlaylistImagesCover.Children[i]).Source = tempSource;
                }
            }
            Channel_name.Content = playlistName;
            _songLabel = songLabel;
            _playlistName = playlistName;
            _backgroundRectangle = backgroundRectangle;
            _youtubePlayer = youtubePlayer;
            _expandedPlaylistHolder = expandedPlaylistHolder;
        }

        private void SearchedPlaylistImagesCover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var playlistContainer = new PlaylistContainerCards(_playlistItemListResponse, _playlistName, _songLabel, _backgroundRectangle, _youtubePlayer );
            if (_firstShow)
            {
                var parent = VisualTreeHelper.GetParent(this);
                ((WrapPanel)parent).Children.Add(playlistContainer);
            }

        }
        private void UserPlaylistCover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var playlistContainer = new PlaylistContainerCards(_playlistItemListResponse, _playlistName, _songLabel, _backgroundRectangle, _youtubePlayer);
            if (_firstShow)
            {
                _expandedPlaylistHolder.Children.Add(playlistContainer);
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("FadeIn")).Begin(Overlay);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("FadeOut")).Begin(Overlay);
        }
    }
}
