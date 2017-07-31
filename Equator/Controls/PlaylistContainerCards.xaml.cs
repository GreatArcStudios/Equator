using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CefSharp.Wpf;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using SuperfastBlur;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for PlaylistContainerCards.xaml
    /// </summary>
    public partial class PlaylistContainerCards : UserControl
    {
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);
        internal PlaylistItemListResponse _playlistItemListResponse;
        internal string _playlistName;
        internal TextBlock _songLabel;
        internal ChromiumWebBrowser _youtubePlayer;
        internal Rectangle _backgroundRectangle;
        public PlaylistContainerCards(PlaylistItemListResponse playlistItemListResponse, string playlistName, TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer)
        {
            InitializeComponent();
            _playlistItemListResponse = playlistItemListResponse;
            _playlistName = playlistName;
            _youtubePlayer = youtubePlayer;
            _songLabel = songLabel;
            _backgroundRectangle = backgroundRectangle;
            Playlist_Title.Content = playlistName;
            Channel_Title.Content = playlistItemListResponse.Items[0].Snippet.ChannelTitle;

            var image = new BitmapImage(new Uri(playlistItemListResponse.Items[0].Snippet.Thumbnails.Medium.Url.ToString()));
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
            BackgroundImage.Source = backgroundImageBrush.ImageSource;
        }
        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/13147707-a9d3-40b9-82e4-290d1c64ccac/bitmapbitmapimage-conversion?forum=wpf
        /// </summary>
        /// <param name="bitImage"></param>
        /// <returns></returns>
        private Bitmap convertBitmap(BitmapImage bitImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return bitmap;
            }
        }
        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => { Container.Opacity = 0;});
        }

        private async void PlaylistItemHolder_Loaded(object sender, RoutedEventArgs e)
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            Console.WriteLine(_playlistName + " has " + _playlistItemListResponse.Items.Count);
            for (int i = 0; i < _playlistItemListResponse.Items.Count; i++)
            {
                Google.Apis.YouTube.v3.Data.PlaylistItem playlistItem = _playlistItemListResponse.Items[i];
                Uri backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.Medium.Url);
                try
                {
                    backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.High.Url);
                }
                finally
                {

                    var request = service.Videos.List("snippet");
                    request.Id = playlistItem.Snippet.ResourceId.VideoId;
                    var response = request.ExecuteAsync();
                    await response;
                    PlaylistItemHolder.Children.Add(new PlaylistItem(backgroundUri,
                        _songLabel, playlistItem.Snippet.ResourceId.VideoId, playlistItem.Snippet.Title, response.Result.Items[0].Snippet.ChannelTitle,
                        _backgroundRectangle, _youtubePlayer, i, _playlistItemListResponse));
                    Console.WriteLine("Added item from " + _playlistName + " " + playlistItem.Snippet.Title);
                }
            }
        }
    }
}
