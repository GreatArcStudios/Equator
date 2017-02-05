using System;
using System.Collections;
using System.Threading.Tasks;
using Equator.Helpers;

namespace Equator.Music
{
    internal class GetSong
    {
        public static string VideoId;

        /// <summary>
        /// Downloads the first song in the Searchlist response list
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        /// TODO: make sure to call QueryVideo.QueryList(song)
        public static async Task AutoPlaySong(string song)
        {
            //use video id to get the song
            VideoId = QueryVideo.SearchListResponse.Items[0].Id.VideoId;
            try
            {
                SongThumb.GetSongThumb(QueryVideo.SearchListResponse.Items[0].Snippet.Thumbnails.Maxres.Url,
                    QueryVideo.SearchListResponse.Items[0].Snippet.Title);
            }
            catch (Exception)
            {
                SongThumb.GetSongThumb(QueryVideo.SearchListResponse.Items[0].Snippet.Thumbnails.High.Url,
                    QueryVideo.SearchListResponse.Items[0].Snippet.Title);
            }
            if (!FilePaths.InCache())
                await Music.GetMusic.DownloadVideo();
        }
    }
}