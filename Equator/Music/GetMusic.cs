using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Equator.Helpers;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NReco.VideoConverter;
using VideoLibrary;
using YoutubeExtractor;

namespace Equator.Music
{
    /// <summary>
    /// The class that gets the particular song
    /// </summary>
    internal class GetMusic
    {
        public static string SongTitle;

        public static async Task<string> DownloadVideo()
        {
           /* // Our test youtube link
            string link = "https://www.youtube.com/watch?v=" + GetSong.VideoId;

            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);

            VideoInfo video = videoInfos
                .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            var fullName = video.Title;
            var saveName = fullName.Replace("- YouTube", "");

            var videoDownloader = new VideoDownloader(video, Path.Combine(FilePaths.SaveLocation(),
                FilePaths.RemoveIllegalPathCharacters(saveName)));
  
            videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

            videoDownloader.Execute();      */

           // Other youtube library libvideo
          var uri = "https://www.youtube.com/watch?v=" + GetSong.VideoId;
          Console.WriteLine(uri);
          var youTube = YouTube.Default;
          var video = youTube.GetVideo(uri);
          var fullName = video.FullName; // same thing as title + fileExtension
          var bytes = await video.GetBytesAsync();
          var stream = video.Stream(); 
         
            var saveName = fullName.Replace("- YouTube", "");
            File.WriteAllBytes(Path.Combine(FilePaths.SaveLocation(),
                FilePaths.RemoveIllegalPathCharacters(saveName)), bytes);     */
            SongTitle = saveName;
            Console.WriteLine("Done downloading " + SongTitle);
            return Path.Combine(FilePaths.SaveLocation(),
                FilePaths.RemoveIllegalPathCharacters(saveName));
        }

        //extract music from the file (currently too slow) 
        public static async void ExtractMusic()
        {
            var filePath = await DownloadVideo();
            var inputFile = new MediaFile {Filename = filePath};
            var outputFile = new MediaFile
            {
                Filename = Path.Combine(FilePaths.SaveLocation(),
                    FilePaths.RemoveIllegalPathCharacters(SongTitle + ".mp3"))
            };
            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }
            File.Delete(filePath);
        }

        public static async Task ConvertWebmToMp4(string inputFilePath, string saveName)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.FFMpegProcessPriority = ProcessPriorityClass.BelowNormal;
            ffMpeg.ConvertMedia(inputFilePath, Path.Combine(FilePaths.SaveLocation(),
                    FilePaths.RemoveIllegalPathCharacters(saveName)), Format.mp4);
            ffMpeg.Stop();
        }
    }
}