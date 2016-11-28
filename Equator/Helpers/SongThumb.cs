using System.Net;

namespace Equator.Helpers
{
    internal class SongThumb
    {
        public static string GetSongThumb(string url, string songName)
        {
            var webClient = new WebClient();
            webClient.DownloadFile(url,
                FilePaths.saveThumb() + "\\" + FilePaths.RemoveIllegalPathCharacters(songName) + ".png");
            return FilePaths.saveThumb() + "\\" + songName + ".png";
        }
    }
}