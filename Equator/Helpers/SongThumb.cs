using System;
using System.Net;

namespace Equator.Helpers
{
    internal class SongThumb
    {
        //public static ArrayList SongThumbUris = new ArrayList();
        public static string GetSongThumb(string url, string songName)
        {
            var filepath = FilePaths.SaveThumb() + "\\" + FilePaths.RemoveIllegalPathCharacters(songName) + ".png";
            try
            {
                var webClient = new WebClient();
                webClient.DownloadFile(url,
                    filepath);
            }
            catch (Exception e)
            {
                return FilePaths.SaveThumb() + "\\" + songName + ".png";
            }
           
            //SongThumbUris.Add(FilePaths.SaveThumb() + "\\" + songName + ".png");
            return FilePaths.SaveThumb() + "\\" + songName + ".png";
        }
    }
}