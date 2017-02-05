using System;
using System.Net;
using System.Windows.Media;
using Equator.Controls;

namespace Equator.Helpers
{
    internal class SongThumb
    {
        public static string GetSongThumb(string url, string songName)
        {
            string filepath = FilePaths.SaveThumb() + "\\" + FilePaths.RemoveIllegalPathCharacters(songName) + ".png";
            var webClient = new WebClient();
            webClient.DownloadFile(url,
               filepath );
            return FilePaths.SaveThumb() + "\\" + songName + ".png";
        }

    }
}