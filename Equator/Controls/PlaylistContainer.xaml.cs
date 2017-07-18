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
using Google.Apis.YouTube.v3.Data;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for PlaylistContainer.xaml 
    /// </summary>
    public partial class PlaylistContainer : UserControl
    {
        /// <summary>
        /// Set the expander label when creating the container
        /// Use PlaylistToPlaylistItems to get the playlistitemresponse
        /// </summary>
        /// <param name="playlistItemListResponse"></param>
        /// <param name="songLabel"></param>
        /// <param name="backgroundRectangle"></param>
        /// <param name="youtubePlayer"></param>
        public PlaylistContainer(PlaylistItemListResponse playlistItemListResponse, string playlistName, TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer)
        {
            InitializeComponent();
            Console.WriteLine(playlistName + " has " + playlistItemListResponse.Items.Count);
            foreach (Google.Apis.YouTube.v3.Data.PlaylistItem playlistItem in playlistItemListResponse.Items)
            {
                Uri backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.Medium.Url);
                try
                {
                    backgroundUri = new Uri(playlistItem.Snippet.Thumbnails.High.Url);
                }
                finally
                {
                    Playlist_Content.Children.Add(new PlaylistItem(backgroundUri,
                        songLabel, playlistItem.Snippet.ResourceId.VideoId, playlistItem.Snippet.Title, playlistItem.Snippet.ChannelTitle,
                        backgroundRectangle, youtubePlayer));
                    Console.WriteLine("Added item from " + playlistName + " " + playlistItem.Snippet.Title);
                } 
            }
            Content_Holder.Header = playlistName;
        }
    }
}
