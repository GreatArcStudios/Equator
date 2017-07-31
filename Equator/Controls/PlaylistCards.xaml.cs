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
using SuperfastBlur;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistCards.xaml
    /// </summary>
    public partial class PlaylistCards : UserControl
    {
        private readonly Rectangle _backgroundRectangle;
        private readonly Grid _expandedPlaylistHolder;
        private bool _firstShow = true;
        private bool _isUserPlaylist;

        private readonly PlaylistItemListResponse _playlistItemListResponse;
        private readonly string _playlistName;
        private readonly TextBlock _songLabel;
        private readonly PlaylistListResponse _userPlaylists;
        private readonly UserPlaylistsContainer _userPlaylistsContainer;
        private readonly ChromiumWebBrowser _youtubePlayer;

        public PlaylistCards(bool userPlaylist, string playlistName, PlaylistListResponse userPlaylistResponse,
            PlaylistItemListResponse playlistItemListResponse, TextBlock songLabel, Rectangle backgroundRectangle,
            ChromiumWebBrowser youtubePlayer, Grid expandedPlaylistHolder)
        {
            InitializeComponent();
            Overlay.Opacity = 0;
            _playlistItemListResponse = playlistItemListResponse;
            _isUserPlaylist = userPlaylist;
            if (userPlaylist)
            {
                _userPlaylists = userPlaylistResponse;
                Console.WriteLine(_userPlaylists);
                //create user image
                ImageBrush userImageBrush;
                if (!File.Exists(FilePaths.SaveUserImage() + "\\Userimage.png"))
                    userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
                else
                    userImageBrush =
                        new ImageBrush(new BitmapImage(new Uri(FilePaths.SaveUserImage() + "\\Userimage.png")));
                userImageBrush.TileMode = TileMode.None;
                UserPlaylistCover.Fill = userImageBrush;
                PlaylistName.Text = "Your Playlists";
                Panel.SetZIndex(SearchedPlaylistImagesCover, -9999);
                Channel_name.Content = _userPlaylists.Items[0].Snippet.ChannelTitle;
                _userPlaylistsContainer = new UserPlaylistsContainer(this);
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    var backgroundImageUri = new Uri(_playlistItemListResponse.Items[i].Snippet.Thumbnails.Medium.Url);
                    ImageSource tempSource = new BitmapImage(backgroundImageUri);
                    ((Image) SearchedPlaylistImagesCover.Children[i]).Source = tempSource;
                }
                Panel.SetZIndex(UserPlaylistCover, -9999);
                Channel_name.Content = playlistItemListResponse.Items[0].Snippet.ChannelTitle;
                PlaylistName.Text = playlistName;
            }

            _songLabel = songLabel;
            _playlistName = playlistName;
            _backgroundRectangle = backgroundRectangle;
            _youtubePlayer = youtubePlayer;
            _expandedPlaylistHolder = expandedPlaylistHolder;
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private void SearchedPlaylistImagesCover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var playlistContainer = new PlaylistContainerCards(_playlistItemListResponse, _playlistName, _songLabel,
                _backgroundRectangle, _youtubePlayer);
            if (_firstShow)
            {
                var parent = VisualTreeHelper.GetParent(this);
                ((WrapPanel) parent).Children.Add(playlistContainer);
            }
        }

        private async void UserPlaylistCover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_firstShow)
            {
                _firstShow = false;
                try
                {
                    File.Copy(FilePaths.SaveUserImage() + "\\Userimage.png",
                        FilePaths.SaveUserImage() + "\\Userimage_temp.png", false);
                }
                catch
                {
                }
                var tempPath = FilePaths.SaveUserImage() + "\\Userimage_temp.png";
                var image = new BitmapImage(new Uri(tempPath));
                var bitmap = convertBitmap(image);
                var blur = new GaussianBlur(bitmap);
                Bitmap blurredThumb = null;
                try
                {
                    blurredThumb = blur.Process(15);
                }
                catch
                {
                    blurredThumb = blur.Process(15);
                }
                bitmap.Dispose();
                var hBitmap = blurredThumb.GetHbitmap();
                var backgroundImageBrush = new ImageBrush();
                backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
                DeleteObject(hBitmap);
                blurredThumb.Dispose();
                backgroundImageBrush.Stretch = Stretch.UniformToFill;
                _backgroundRectangle.Fill = backgroundImageBrush;
                try
                {
                    File.Delete(FilePaths.SaveUserImage() + "\\Userimage_temp.png");
                }
                catch
                {
                    Console.WriteLine("Deleting temp user image failed");
                }
                foreach (var userPlaylistResponse in _userPlaylists.Items)
                {
                    var playlistItems = await Playlists.PlaylistToPlaylistItems(userPlaylistResponse.Id);
                    _userPlaylistsContainer.PlaylistItemHolder.Children.Add(new PlaylistCards(false,
                        userPlaylistResponse.Snippet.Title, null, playlistItems, _songLabel, _backgroundRectangle,
                        _youtubePlayer, _expandedPlaylistHolder));
                }
                Panel.SetZIndex(this, -9999);
                await Dispatcher.InvokeAsync(() => { _expandedPlaylistHolder.Children.Add(_userPlaylistsContainer); });
            }
            else
            {
                Panel.SetZIndex(this, -9999);
                await Dispatcher.InvokeAsync(() => { _userPlaylistsContainer.Container.Opacity = 100; });
                var tempPath = FilePaths.SaveUserImage() + "\\Userimage_temp.png";
                var image = new BitmapImage(new Uri(tempPath));
                var bitmap = convertBitmap(image);
                var blur = new GaussianBlur(bitmap);
                Bitmap blurredThumb = null;
                try
                {
                    blurredThumb = blur.Process(15);
                }
                catch
                {
                    blurredThumb = blur.Process(15);
                }
                bitmap.Dispose();
                var hBitmap = blurredThumb.GetHbitmap();
                var backgroundImageBrush = new ImageBrush();
                backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
                DeleteObject(hBitmap);
                blurredThumb.Dispose();
                backgroundImageBrush.Stretch = Stretch.UniformToFill;
                _backgroundRectangle.Fill = backgroundImageBrush;
            }
        }

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
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard) FindResource("FadeIn")).Begin(Overlay);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard) FindResource("FadeOut")).Begin(Overlay);
        }
    }
}