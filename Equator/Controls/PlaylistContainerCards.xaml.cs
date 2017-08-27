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
using Equator.Music;
using Google.Apis.YouTube.v3.Data;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for PlaylistContainerCards.xaml
    /// </summary>
    public partial class PlaylistContainerCards : UserControl
    {
        internal readonly Rectangle BackgroundRectangle;
        internal readonly PlaylistItemListResponse PlaylistItemListResponse;
        internal readonly string PlaylistName;
        internal readonly TextBlock SongLabel;
        internal readonly ChromiumWebBrowser YoutubePlayer;
        private readonly PlaylistCards _parentCard;
        private readonly ScrollViewer _playlistScrollViewer;
        private bool _firstShow = true;
        private Button _playButton;
        public PlaylistContainerCards(PlaylistItemListResponse playlistItemListResponse, string playlistName,
            TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer, PlaylistCards parentCard, ScrollViewer playlistScrollViewer, Button playButton)
        {
            InitializeComponent();
            PlaylistItemListResponse = playlistItemListResponse;
            PlaylistName = playlistName;
            YoutubePlayer = youtubePlayer;
            SongLabel = songLabel;
            BackgroundRectangle = backgroundRectangle;
            PlaylistTitle.Text = playlistName;
            ChannelTitle.Text = playlistItemListResponse.Items[0].Snippet.ChannelTitle;
            _parentCard = parentCard;
            _playlistScrollViewer = playlistScrollViewer;
            _playButton = playButton;
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
                    Panel.SetZIndex(_playlistScrollViewer, 3);
                    _playlistScrollViewer.Opacity = 100;                
                    /*
                    if (((Grid) VisualTreeHelper.GetParent(this)).Children.Count > 4)
                    {
                        for (int i = 0; i < ((Grid)VisualTreeHelper.GetParent(this)).Children.Count - 1; i++)
                        {
                            ((Grid)VisualTreeHelper.GetParent(this)).Children.RemoveAt(i);
                            Console.WriteLine(((Grid)VisualTreeHelper.GetParent(this)).Children.Count);
                        }
                    }*/


                }
            });
        }

        private async void PlaylistItemHolder_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstShow)
            {
                _firstShow = false;
                var service = GoogleServices.YoutubeService;
                Console.WriteLine(PlaylistName + " has " + PlaylistItemListResponse.Items.Count);
                for (var i = 0; i < PlaylistItemListResponse.Items.Count; i++)
                {
                    var playlistItem = PlaylistItemListResponse.Items[i];
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
                            SongLabel, playlistItem.Snippet.ResourceId.VideoId, playlistItem.Snippet.Title,
                            response.Result.Items[0].Snippet.ChannelTitle,
                            BackgroundRectangle, YoutubePlayer, i, PlaylistItemListResponse, _playButton));
                        Console.WriteLine("Added item from " + PlaylistName + " " + playlistItem.Snippet.Title);
                    }
                }
            }
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            var firstChild = (PlaylistItem)PlaylistItemHolder.Children[0];
            MusicPanel.PlayListIndex = firstChild.Index;
            MusicPanel.PlayedPlaylistIndiciesBackwards.Add(firstChild.Index);
            MusicPanel.PlayingSongs = false;
            Console.WriteLine("Playing Songs? " + MusicPanel.PlayingSongs);
            QueryYoutube.CurrentPlaylistItemListResponse = firstChild.ParentPlaylist;
            await Music.Music.PlaySpecifiedSong(BackgroundRectangle, firstChild.MusicLink, firstChild.SongTitle.Text, SongLabel, YoutubePlayer,
                firstChild.BackgroundImageUrl, _playButton);
        }
    }
}