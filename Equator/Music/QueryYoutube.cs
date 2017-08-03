using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Helpers
{
    internal static class QueryYoutube
    {
        public static SearchListResponse SearchListResponse;

        //keep the list of videos separate from the playlists
        public static List<string> videos = new List<string>();

        //public static SearchListResponse TopicSearchListResponse;
        //public static string CurrentSongTitle; 
        public static int SongCount { get; set; } = 50;

        /// <summary>
        ///     Gets a list of songs of size <c>int SongCount</c>
        /// </summary>
        /// <param name="song"></param>
        public static async Task<int> QueryVideoListAsync(string song)
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var musicList = service.Search.List("snippet");
            musicList.Q = song; // Replace with your search term.
            musicList.MaxResults = SongCount;
            musicList.Type = "video";

            // Call the search.list method to retrieve results matching the specified query term.
            SearchListResponse = await musicList.ExecuteAsync();
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
            musicList.Q = song + " topic"; // Replace with your search term.
            musicList.MaxResults = 1;
            musicList.Type = "video";
            var topicResponse = await musicList.ExecuteAsync();
            SearchListResponse.Items.Insert(0, topicResponse.Items[0]);
            //TopicSearchListResponse = await musicList.ExecuteAsync();
            return 1;
        }

        public static void QueryPlaylistList(string song)
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var musicList = service.Search.List("snippet");
            musicList.Q = song; // Replace with your search term.
            musicList.MaxResults = SongCount;
            musicList.Type = "playlist";

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

        public static void QueryChannelList(string song)
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
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
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, true, GoogleServices.Credential);
            var playlistItemRequest = service.PlaylistItems.List("snippet");
            playlistItemRequest.PlaylistId = playlistId;
            playlistItemRequest.MaxResults = 50;
            var response = await playlistItemRequest.ExecuteAsync();
            return response;
        }

        public static async Task<PlaylistListResponse> GetUserPlaylist()
        {
            await GoogleServices.AuthUserCredential();
            var credential = GoogleServices.Credential;
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, true, credential);
            var userPlaylistRequest = service.Playlists.List("snippet");
            userPlaylistRequest.Mine = true;
            var userPlaylistResponse = await userPlaylistRequest.ExecuteAsync();
            Console.WriteLine("First id response from getuserplaylist " + userPlaylistResponse.Items[0].Id);
            return userPlaylistResponse;
        }

        /*
           Get top 50 songs from this playlist: https://www.youtube.com/playlist?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG
        */
        public static PlaylistItemListResponse GetTopSongs()
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;
        }

        public static PlaylistItemListResponse GetNewSongs()
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLvFYFNbi-IBFeP5ALr50hoOmKiYRMvzUq";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;
        }
    }
}