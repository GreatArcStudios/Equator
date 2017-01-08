using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MediaToolkit;
using MediaToolkit.Model;
using YoutubeExtractor;

namespace Equator.Music
{
    class GetMusic
    { 
         static IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=" + GetSong.VideoID, false);
         public static string SongTitle;
        
        private static string DownloadVideo(IEnumerable<VideoInfo> videoInfos)
         {
        
            VideoInfo video = videoInfos
                .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 0);
          
            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the video downloader.
             * The first argument is the video to download.
             * The second argument is the path to save the video file.
             */
            var videoDownloader = new VideoDownloader(video,
                Path.Combine(Helpers.FilePaths.SaveLocation() +"\\" + video.Title + video.VideoExtension));

            // Register the ProgressChanged event and print the current progress
            videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

            /*
             * Execute the video downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            videoDownloader.Execute();
            SongTitle = video.Title;
            return Path.Combine(Helpers.FilePaths.SaveLocation(),
                 RemoveIllegalPathCharacters(video.Title) + video.VideoExtension);
        }
        private static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
        //extract music from the file
        public static void ExtractMusic()
        {
            string filePath = DownloadVideo(videoInfos);
            var inputFile = new MediaFile { Filename = filePath };
            var outputFile = new MediaFile { Filename = Path.Combine(Helpers.FilePaths.SaveLocation(),
                RemoveIllegalPathCharacters(SongTitle + ".mp3")) };
            using (var engine = new Engine())
            {
                engine.Convert(inputFile,outputFile);
            }
            File.Delete(filePath);
        }
    }

}
