using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Helpers
{
    class QueryVideo
    {
        public static int songCount = 10;
        public static SearchListResponse searchListResponse;
        public static void QueryList(string song)
        {
            List<string> videos = new List<string>();
            
            var service = AuthGoogle.CreateService(AuthGoogle.ApiKey, false, null);
            var musicList = service.Search.List("snippet");
            musicList.Q = song; // Replace with your search term.
            musicList.MaxResults = songCount;

            // Call the search.list method to retrieve results matching the specified query term.
            searchListResponse =  musicList.Execute();
        } 
    }
}
