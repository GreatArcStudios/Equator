using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CefSharp.Wpf;
using Equator.Helpers;
using Equator.Music;
using Google.Apis.YouTube.v3.Data;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistCards.xaml
    /// </summary>
    public partial class PlaylistCards : UserControl
    {
        internal readonly UserPlaylistsContainer UserPlaylistsContainer;
        private readonly Rectangle _backgroundRectangle;
        private readonly Grid _expandedPlaylistHolder;
        private bool _firstShow = true;
        private bool _isUserPlaylist;
        private PlaylistContainerCards _playlistContainerCards;
        private readonly Button _playButton;
        private readonly PlaylistItemListResponse _playlistItemListResponse;
        private readonly string _playlistName;
        private readonly TextBlock _songLabel;
        private readonly PlaylistListResponse _userPlaylists;
        private readonly ChromiumWebBrowser _youtubePlayer;
        private readonly ScrollViewer _playlistScrollViewer;
        public PlaylistCards(bool userPlaylist, bool userPlaylistContainerParent, string playlistName, PlaylistListResponse userPlaylistResponse,
            PlaylistItemListResponse playlistItemListResponse, TextBlock songLabel, Rectangle backgroundRectangle,
            ChromiumWebBrowser youtubePlayer, Grid expandedPlaylistHolder, UserPlaylistsContainer userPlaylistsContainer, ScrollViewer
             playlistScrollViewer, Button playButton)
        {
            InitializeComponent();
            Overlay.Opacity = 0;
            _playlistItemListResponse = playlistItemListResponse;
            _isUserPlaylist = userPlaylist;
            _playlistScrollViewer = playlistScrollViewer;
            _playButton = playButton;
            if (userPlaylistContainerParent)
            {
                UserPlaylistsContainer = userPlaylistsContainer;
            }
            if (userPlaylist)
            {
                _userPlaylists = userPlaylistResponse;
                Console.WriteLine(_userPlaylists);
                //create user image
                ImageBrush userImageBrush;
                if (!File.Exists(FilePaths.UserImageLocation + "\\Userimage.png"))
                    userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
                else
                    userImageBrush =
                        new ImageBrush(new BitmapImage(new Uri(FilePaths.UserImageLocation + "\\Userimage.png")));
                userImageBrush.TileMode = TileMode.None;
                UserPlaylistCover.Fill = userImageBrush;
                PlaylistName.Text = "Your Playlists";
                Panel.SetZIndex(SearchedPlaylistImagesCover, -9999);
                ChannelName.Content = _userPlaylists.Items[0].Snippet.ChannelTitle;
                UserPlaylistsContainer = new UserPlaylistsContainer(this, _playlistScrollViewer);

            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    Uri backgroundImageUri;
                    try
                    {
                        backgroundImageUri = new Uri(_playlistItemListResponse.Items[i].Snippet.Thumbnails.Medium.Url);
                        ImageSource tempSource = new BitmapImage(backgroundImageUri);
                        ((Image)SearchedPlaylistImagesCover.Children[i]).Source = tempSource;
                    }
                    catch
                    {
                        Console.WriteLine("Adding song failed");
                    }
                }
                Panel.SetZIndex(UserPlaylistCover, -9999);
                ChannelName.Content = playlistItemListResponse.Items[0].Snippet.ChannelTitle;
                PlaylistName.Text = playlistName;
            }
            Panel.SetZIndex(this, 1);
            _songLabel = songLabel;
            _playlistName = playlistName;
            _backgroundRectangle = backgroundRectangle;
            _youtubePlayer = youtubePlayer;
            _expandedPlaylistHolder = expandedPlaylistHolder;
            _isUserPlaylist = userPlaylist;
        }
        /*
        /// <summary>
        ///     https://social.msdn.microsoft.com/Forums/vstudio/en-US/13147707-a9d3-40b9-82e4-290d1c64ccac/bitmapbitmapimage-conversion?forum=wpf
        /// </summary>
        /// <param name="bitImage"></param>
        /// <returns></returns>
        private Bitmap convertBitmap(BitmapImage bitImage)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitImage));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);
                return bitmap;
            }
        }*/

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("FadeIn")).Begin(Overlay);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("FadeOut")).Begin(Overlay);
        }

        private async void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Playlist Card First Show? " + _firstShow);
            if (_firstShow)
            {
                if (_isUserPlaylist)
                {
                    foreach (var userPlaylistResponse in _userPlaylists.Items)
                    {
                        var playlistItems = await QueryYoutube.PlaylistToPlaylistItems(userPlaylistResponse.Id);
                        UserPlaylistsContainer.PlaylistItemHolder.Children.Add(new PlaylistCards(false, true,
                            userPlaylistResponse.Snippet.Title, null, playlistItems, _songLabel, _backgroundRectangle,
                            _youtubePlayer, _expandedPlaylistHolder, UserPlaylistsContainer, _playlistScrollViewer, _playButton));
                    }
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _expandedPlaylistHolder.Children.Add(UserPlaylistsContainer);
                        Panel.SetZIndex(_playlistScrollViewer, -9999);
                        _playlistScrollViewer.Opacity = 0;
                        Panel.SetZIndex(UserPlaylistsContainer, 3);
                    });
                }
                else if (UserPlaylistsContainer != null)
                {
                    _playlistContainerCards = new PlaylistContainerCards(_playlistItemListResponse, _playlistName, _songLabel, _backgroundRectangle, _youtubePlayer, this, _playlistScrollViewer,_playButton);
                    Panel.SetZIndex(_playlistContainerCards, 4);
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _expandedPlaylistHolder.Children.Add(_playlistContainerCards);
                        UserPlaylistsContainer.Opacity = 0;
                    });
                }
                else
                {
                    _playlistContainerCards = new PlaylistContainerCards(_playlistItemListResponse, _playlistName, _songLabel, _backgroundRectangle, _youtubePlayer, this, _playlistScrollViewer, _playButton);
                    Panel.SetZIndex(_playlistContainerCards, 4);
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _expandedPlaylistHolder.Children.Add(_playlistContainerCards);
                        Panel.SetZIndex(_playlistScrollViewer, -9999);
                        _playlistScrollViewer.Opacity = 0;
                    });
                }
                _firstShow = false;
            }
            else
            {

                if (_isUserPlaylist)
                    await Dispatcher.InvokeAsync(() =>
                    {
                        Panel.SetZIndex(_playlistScrollViewer, -9999);
                        _playlistScrollViewer.Opacity = 0;
                        UserPlaylistsContainer.Opacity = 100;
                        Panel.SetZIndex(UserPlaylistsContainer, 3);
                        UserPlaylistsContainer.IsEnabled = true;
                    });
                else if (UserPlaylistsContainer != null)
                    await Dispatcher.InvokeAsync(() =>
                    {
                        Panel.SetZIndex(UserPlaylistsContainer, -9999);
                        UserPlaylistsContainer.Opacity = 0;
                        Panel.SetZIndex(_playlistContainerCards, 4);
                        _playlistContainerCards.Opacity = 100;
                        _playlistContainerCards.IsEnabled = true;
                    });
                else
                    await Dispatcher.InvokeAsync(() =>
                    {
                        Panel.SetZIndex(_playlistScrollViewer, -9999);
                        _playlistScrollViewer.Opacity = 0;
                        Panel.SetZIndex(_playlistContainerCards, 4);
                        _playlistContainerCards.Opacity = 100;
                        _playlistContainerCards.IsEnabled = true;
                    });
            }

        }
    }
}