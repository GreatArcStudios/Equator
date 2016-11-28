using System;
using System.IO;
using System.Text.RegularExpressions;
using Equator.Music;
using VideoLibrary;

namespace Equator.Helpers
{
    internal class FilePaths
    {
        private static readonly string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                      "\\Equator Music\\cache";

        private static readonly string thumbLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\cache\\images";

        public static string SaveLocation()
        {
            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);
            return saveLocation;
        }

        public static string saveThumb()
        {
            if (!Directory.Exists(thumbLocation))
                Directory.CreateDirectory(thumbLocation);
            return thumbLocation;
        }

        public static string RemoveIllegalPathCharacters(string path)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        public static bool InCache()
        {
            var uri = "https://www.youtube.com/watch?v=" + GetSong.VideoID;
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(uri);
            var fullName = video.FullName;
            var saveName = fullName.Replace("- YouTube", "");
            if (!File.Exists(Path.Combine(SaveLocation(),
                RemoveIllegalPathCharacters(saveName))))
                return false;
            return true;
        }
    }
}