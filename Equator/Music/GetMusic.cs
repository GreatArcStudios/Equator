#define USE_YOUTUBEEXPLODE
using System;
using System.Threading.Tasks;
using CefSharp;
using System.Linq;
using YoutubeExplode;
using CefSharp.Wpf;

namespace Equator.Music
{
    /// <summary>
    ///     The class that gets the particular song
    /// </summary>
    
    internal class GetMusic
    {
#if OFFLINE_IMPLEMENTED
        public static string SongTitle;
        private static bool isConverting;
        public static FFMpegConverter FFMpeg = new FFMpegConverter();

        public static bool IsConverting
        {
            get
            {
                return isConverting;
            }

            set
            {
                isConverting = value;
            }
        }
#endif
        public static async Task<string> DownloadVideo(ChromiumWebBrowser youtubePlayer)
        {
#if USE_YOUTUBE_EXTRACTOR
            string link = "https://www.youtube.com/watch?v=" + GetSong.VideoId;
 
             IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
 
             VideoInfo video = videoInfos
                 .First(info => info.VideoType == VideoType.WebM && info.Resolution == 360);
            
             if (video.RequiresDecryption)
             {
                 DownloadUrlResolver.DecryptDownloadUrl(video);
             }
            
            var fullName = video.Title;
             var saveName = fullName.Replace("- YouTube", "");
 
             var videoDownloader = new VideoDownloader(video, Path.Combine(FilePaths.SaveLocation(),
                 FilePaths.RemoveIllegalPathCharacters(saveName)));
   
             videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);
 
             videoDownloader.Execute();
#endif
#if USE_LIBVIDEO
            /*// Other youtube library libvideo
            var uri = "https://www.youtube.com/watch?v=" + GetSong.VideoId;
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(uri);*/

            //youtubePlayer.Address = "http://33232463.nhd.weebly.com/";
            /*var fullName = video.FullName; // same thing as title + fileExtension
            var saveName = FilePaths.RemoveIllegalPathCharacters(fullName.Replace("- YouTube", ""));
            saveName = saveName.Replace("_", "");
            var bytes = await video.GetBytesAsync();
            //var stream = video.Stream();*/
#endif
#if USE_YOUTUBEEXPLODE
            // Client
            var client = new YoutubeClient();
            var videoInfo = await client.GetVideoInfoAsync(GetSong.VideoId);
            // Print metadata
            Console.WriteLine($"Id: {videoInfo.Id} | Title: {videoInfo.Title} | Author: {videoInfo.Author.Title}");

            // Get the most preferable stream
            Console.WriteLine("Looking for the best mixed stream...");
            var streamInfo = videoInfo.MixedStreams
                .OrderBy(s => s.VideoEncoding == YoutubeExplode.Models.MediaStreams.VideoEncoding.Vp8)
                .Last();
            youtubePlayer.LoadHtml("<html><body scroll=\"no\" style=\"overflow: hidden\"><video id = \"youtubePlayer\" height = \"270\" width = \"480\" autoplay>" + "<source src=\"" + streamInfo.Url + "\" type=\"video/webm\"></source><html>", "http://rendering");
            Console.WriteLine("Player loaded? " + youtubePlayer.IsLoaded);
            //Console.WriteLine("Can execute JS: "+ youtubePlayer.CanExecuteJavascriptInMainFrame);
#if OFFLINE_IMPLEMENTED
            streamInfo = videoInfo.MixedStreams
               .OrderBy(s => s.VideoEncoding == YoutubeExplode.Models.mediaStreams.VideoEncoding.H264)
               .Last();
            // Compose file name, based on metadata
            string fileExtension = streamInfo.Container.GetFileExtension();
            string saveName = $"{videoInfo.Title}.{fileExtension}";
            // Remove illegal characters from file name
            saveName = FilePaths.RemoveIllegalPathCharacters(saveName);
            // Download video
            Console.WriteLine($"Downloading to [{saveName}]...");
            string savePath = Path.Combine(FilePaths.SaveLocation(),

               saveName);
            await client.DownloadmediaStreamAsync(streamInfo, savePath);


            //File.WriteAllBytes(savePath, bytes);
            SongTitle = savePath;
            Console.WriteLine("Done downloading " + SongTitle);
            return savePath;
#endif
#endif
            return videoInfo.Title;
        }
        /*
        //extract music from the file (currently too slow) 
        public static async void ExtractMusic(ChromiumWebBrowser youtubePlayer)
        {
            var filePath = await DownloadVideo(youtubePlayer);
            var inputFile = new mediaFile {Filename = filePath};
            var outputFile = new mediaFile
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
        */
#if OFFLINE_IMPLEMENTED
        public static async Task ConvertWebmToMp4(string inputFilePath, string saveName)
        {
            FFMpeg.FFMpegProcessPriority = ProcessPriorityClass.RealTime;
            isConverting = true;
            await Task.Run(async () => { await ConvertTask(FFMpeg, inputFilePath, saveName); });
        }

        private static async Task ConvertTask(FFMpegConverter ffMpegConverter, string inputFilePath, string saveName)
        {
            ffMpegConverter.Convertmedia(inputFilePath, Path.Combine(FilePaths.SaveLocation(),
                FilePaths.RemoveIllegalPathCharacters(saveName)), Format.mp4);
            ffMpegConverter.Stop();
            isConverting = false;
        }
#endif

    }
}