using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Helpers
{
    internal static class QueryYoutube
    {
        public static SearchListResponse SearchListResponse;
        //public static string CurrentSongTitle; 
        public static int SongCount { get; set; } = 50;
        //keep the list of videos separate from the playlists
        public static List<string> videos = new List<string>();

        /// <summary>
        ///     Gets a list of songs of size <c>int SongCount</c>
        /// </summary>
        /// <param name="song"></param>
        public static void QueryVideoList(string song)
        {
            

            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var musicList = service.Search.List("snippet");
            musicList.Q = song; // Replace with your search term.
            musicList.MaxResults = SongCount;
            musicList.Type = "video";

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