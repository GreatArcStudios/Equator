using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using CefSharp.Wpf;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistContainer.xaml
    /// </summary>
    public partial class PlaylistContainer : UserControl
    {
        internal PlaylistItemListResponse _playlistItemListResponse;

        /// <summary>
        ///     Set the expander label when creating the container
        ///     Use PlaylistToPlaylistItems to get the playlistitemresponse
        /// </summary>
        /// <param name="playlistItemListResponse"></param>
        /// <param name="songLabel"></param>
        /// <param name="backgroundRectangle"></param>
        /// <param name="youtubePlayer"></param>
        public PlaylistContainer(PlaylistItemListResponse playlistItemListResponse, string playlistName,
            TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer)
        {
            _playlistItemListResponse = playlistItemListResponse;
            InitializeComponent();
            Console.WriteLine(playlistName + " has " + playlistItemListResponse.Items.Count);
            for (var i = 0; i < playlistItemListResponse.Items.Count; i++)
            {
                var playlistItem = playlistItemListResponse.Items[i];
                var backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.Medium.Url);
                try
                {
                    backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.High.Url);
                }
                finally
                {
                    var service = GoogleServices.YoutubeService;
                    var request = service.Videos.List("snippet");
                    request.Id = playlistItem.Snippet.ResourceId.VideoId;
                    var response = request.Execute();
                    Playlist_Content.Children.Add(new PlaylistItem(backgroundUri,
                        songLabel, playlistItem.Snippet.ResourceId.VideoId, playlistItem.Snippet.Title,
                        response.Items[0].Snippet.ChannelTitle,
                        backgroundRectangle, youtubePlayer, i, _playlistItemListResponse));
                    Console.WriteLine("Added item from " + playlistName + " " + playlistItem.Snippet.Title);
                }
            }
            Content_Holder.Header = playlistName;
        }
    }
}