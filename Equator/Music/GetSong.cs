using System;
using System.Collections;
using System.Threading.Tasks;
using Equator.Helpers;

namespace Equator.Music
{
    internal class GetSong
    {
        public static string VideoID;
        public static async Task GetMusic(string song)
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
            if (!FilePaths.InCache())
                await Music.GetMusic.DownloadVideo();
        }
    }
}