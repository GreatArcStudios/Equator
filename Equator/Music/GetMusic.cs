using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Equator.Helpers;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NReco.VideoConverter;
using VideoLibrary;

namespace Equator.Music
{
    internal class GetMusic
    {
        public static string SongTitle;

        public static async Task<string> DownloadVideo()
        {
            var uri = "https://www.youtube.com/watch?v=" + GetSong.VideoID;
            Console.WriteLine(uri);
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(uri);
            var fullName = video.FullName; // same thing as title + fileExtension
            var bytes = await video.GetBytesAsync();
            var stream = video.Stream();
            var saveName = fullName.Replace("- YouTube", "");
            File.WriteAllBytes(Path.Combine(FilePaths.SaveLocation(),
                FilePaths.RemoveIllegalPathCharacters(saveName)), bytes);
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