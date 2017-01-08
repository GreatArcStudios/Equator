using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Music
{
    class GetSong
    {

        public static string VideoID;

        public async static void GetMusic(string song)
        {
          
            QueryVideo.QueryList(song);
            //use video id to get the song
            VideoID = QueryVideo.searchListResponse.Items[0].Id.VideoId;
            try
            {
                SongThumb.GetSongThumb(QueryVideo.searchListResponse.Items[0].Snippet.Thumbnails.Maxres.Url,
                QueryVideo.searchListResponse.Items[0].Snippet.Title);
            }
            catch (Exception)
            {
                SongThumb.GetSongThumb(QueryVideo.searchListResponse.Items[0].Snippet.Thumbnails.High.Url,
                QueryVideo.searchListResponse.Items[0].Snippet.Title);
            }    
            
            Music.GetMusic.ExtractMusic();
        }
    }
}
