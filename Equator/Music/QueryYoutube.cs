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
    }
}