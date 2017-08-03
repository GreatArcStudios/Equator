using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CefSharp.Wpf;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;
using SuperfastBlur;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistContainerCards.xaml
    /// </summary>
    public partial class PlaylistContainerCards : UserControl
    {
        internal Rectangle _backgroundRectangle;
        internal PlaylistItemListResponse _playlistItemListResponse;
        internal string _playlistName;
        internal TextBlock _songLabel;
        internal ChromiumWebBrowser _youtubePlayer;
        private PlaylistCards _parentCard;

        public PlaylistContainerCards(PlaylistItemListResponse playlistItemListResponse, string playlistName,
            TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer, PlaylistCards parentCard)
        {
            InitializeComponent();
            _playlistItemListResponse = playlistItemListResponse;
            _playlistName = playlistName;
            _youtubePlayer = youtubePlayer;
            _songLabel = songLabel;
            _backgroundRectangle = backgroundRectangle;
            Playlist_Title.Content = playlistName;
            Channel_Title.Content = playlistItemListResponse.Items[0].Snippet.ChannelTitle;
            _parentCard = parentCard;
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                this.Opacity = 0;
                Panel.SetZIndex(this, -9999);
                IsEnabled = false;
                if (_parentCard.UserPlaylistsContainer != null)
                {
                    _parentCard.Opacity = 100;
                    _parentCard.UserPlaylistsContainer.Opacity = 100;
                    Panel.SetZIndex(_parentCard.UserPlaylistsContainer, 3);
                }
                else
                {
                    ((WrapPanel) VisualTreeHelper.GetParent(_parentCard)).Opacity = 100;
                    Panel.SetZIndex(((WrapPanel)VisualTreeHelper.GetParent(_parentCard)), 3);
                }
            });
        }

        private async void PlaylistItemHolder_Loaded(object sender, RoutedEventArgs e)
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            Console.WriteLine(_playlistName + " has " + _playlistItemListResponse.Items.Count);
            for (var i = 0; i < _playlistItemListResponse.Items.Count; i++)
            {
                var playlistItem = _playlistItemListResponse.Items[i];
                var backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.Medium.Url);
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
                        _songLabel, playlistItem.Snippet.ResourceId.VideoId, playlistItem.Snippet.Title,
                        response.Result.Items[0].Snippet.ChannelTitle,
                        _backgroundRectangle, _youtubePlayer, i, _playlistItemListResponse));
                    Console.WriteLine("Added item from " + _playlistName + " " + playlistItem.Snippet.Title);
                }
            }
        }
    }
}