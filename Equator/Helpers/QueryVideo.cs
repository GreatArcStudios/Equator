using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Helpers
{
    internal class QueryVideo
    {
        public static int songCount = 10;
        public static SearchListResponse searchListResponse;

        public static void QueryList(string song)
        {
            var videos = new List<string>();

            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var musicList = service.Search.List("snippet");
            musicList.Q = song; // Replace with your search term.
            musicList.MaxResults = songCount;

            // Call the search.list method to retrieve results matching the specified query term.
            searchListResponse = musicList.Execute();
        }
    }
}