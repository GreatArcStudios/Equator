using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Music
{
    internal static class QueryYoutube
    {
        public static SearchListResponse SearchListResponse;

        //keep the list of videos separate from the playlists
        public static List<string> videos = new List<string>();

        //public static SearchListResponse TopicSearchListResponse;
        //public static string CurrentSongTitle; 
        public static int SongCount { get; set; } = 50;
        public static int PlaylistCount { get; set; } = 20;
        /// <summary>
        ///     Gets a list of songs of size <c>int SongCount</c>
        /// </summary>
        /// <param name="song"></param>
        public static async Task<int> QueryVideoListAsync(string song)
        {
            //Catches timeouts
            try
            {
                var service = GoogleServices.YoutubeService;
                var musicList = service.Search.List("snippet");
                musicList.Q = song; // Replace with your search term.
                musicList.MaxResults = SongCount;
                musicList.Type = "video";

                // Call the search.list method to retrieve results matching the specified query term.
                SearchListResponse = await musicList.ExecuteAsync();
                //Search for topic songs
                musicList.Q = song + " topic"; // Replace with your search term.
                musicList.MaxResults = 1;
                musicList.Type = "video";
                var topicResponse = await musicList.ExecuteAsync();
                SearchListResponse.Items.Insert(0, topicResponse.Items[0]);
            }
            catch
            {
                Console.WriteLine("Timed out");
            }

            //TopicSearchListResponse = await musicList.ExecuteAsync();
            return 1;
        }

        public static async Task QueryPlaylistList(string song)
        {
            try
            {
                var service = GoogleServices.YoutubeService;
                var musicList = service.Search.List("snippet");
                musicList.Q = song; // Replace with your search term.
                musicList.MaxResults = SongCount;
                musicList.Type = "playlist";
                // Call the search.list method to retrieve results matching the specified query term.
                SearchListResponse = await musicList.ExecuteAsync();
            }
            catch
            {
                Console.WriteLine("Timed out");
            }
         
        }

        public static void QueryChannelList(string song)
        {
            var service = GoogleServices.YoutubeService;
            var musicList = service.Search.List("snippet");
            musicList.Q = song; // Replace with your search term.
            musicList.MaxResults = SongCount;
            musicList.Type = "channel";

            // Call the search.list method to retrieve results matching the specified query term.
            SearchListResponse = musicList.Execute();
            try
            {
                Console.WriteLine("Queryed Youtube for SearchListResponse and SearchListResponse created with " +
                                  SearchListResponse.Items.Count + " items" + "Fist item is: " +
                                  SearchListResponse.Items[0].Id.VideoId);
            }
            catch
            {
                Console.WriteLine("Query Failed");
            }
        }
        public static PlaylistItemListResponse CurrentPlaylistItemListResponse;

        /**  NOT DONE TOPIC PLAYIST ISSUES 
        public static void GetTopicPlayist()
        {
            string channelId = QueryYoutube.searchListResponse.Items[0].Id.ChannelId;

        }  **/
        public static async Task<PlaylistItemListResponse> PlaylistToPlaylistItems(string playlistId)
        {
            var service = GoogleServices.YoutubeService;
            var playlistItemRequest = service.PlaylistItems.List("snippet");
            playlistItemRequest.PlaylistId = playlistId;
            playlistItemRequest.MaxResults = PlaylistCount;
            var response = await playlistItemRequest.ExecuteAsync();
            return response;
        }

        public static async Task<PlaylistListResponse> QueryUserPlaylists()
        {
            var service = GoogleServices.YoutubeService;
            var userPlaylistRequest = service.Playlists.List("snippet");
            userPlaylistRequest.Mine = true;
            var userPlaylistResponse = await userPlaylistRequest.ExecuteAsync();
            Console.WriteLine("First id response from getuserplaylist " + userPlaylistResponse.Items[0].Id);
            return userPlaylistResponse;
        }

        /*
           Get top 50 songs from this playlist: https://www.youtube.com/playlist?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG
        */
        public static PlaylistItemListResponse QueryTopSongs()
        {
            var service = GoogleServices.YoutubeService;
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;
        }

        public static PlaylistItemListResponse QueryNewSongs()
        {
            var service = GoogleServices.YoutubeService;
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLvFYFNbi-IBFeP5ALr50hoOmKiYRMvzUq";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;
        }
    }
}