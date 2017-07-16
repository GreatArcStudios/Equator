using System;
using System.IO;
using System.Text.RegularExpressions;
using Equator.Music;
//using VideoLibrary;

namespace Equator.Helpers
{
    internal static class FilePaths
    {
        public static readonly string DefaultImageLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\userdata\\images\\DefaultImage.png";
        private static readonly string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                      "\\Equator Music\\cache";

        private static readonly string ThumbLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\cache\\images";

        private static readonly string UserImageLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\userdata\\images";

        private static readonly string UserCredLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\userdata\\credentials";
        
        public static string SaveLocation()
        {
            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);
            return saveLocation;
        }

        public static string SaveThumb()
        {
            if (!Directory.Exists(ThumbLocation))
                Directory.CreateDirectory(ThumbLocation);
            return ThumbLocation;
        }

        public static string SaveUserImage()
        {
            if (!Directory.Exists(UserImageLocation))
                Directory.CreateDirectory(UserImageLocation);
            return UserImageLocation;
        }

        public static string SaveUserCreds()
        {
            if (!Directory.Exists(UserCredLocation))
                Directory.CreateDirectory(UserCredLocation);
            return UserCredLocation;
        }

        public static string RemoveIllegalPathCharacters(string path)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            GC.Collect();
            return r.Replace(path, "");
        }
#if OFFLINE_IMPLEMENTED
        public static bool InCache()
        {
            var uri = "https://www.youtube.com/watch?v=" + GetSong.VideoId;
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(uri);
            var fullName = video.FullName;
            var saveName = RemoveIllegalPathCharacters(fullName.Replace("- YouTube", ""));
            saveName = saveName.Replace("_", "");
            if (!File.Exists(Path.Combine(SaveLocation(),
                saveName)))
                return false;
            return true;
        }
#endif
    }
}