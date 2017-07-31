#define USE_YOUTUBEEXPLODE
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Equator.Helpers;
using Path = System.IO.Path;
using System.Drawing;
using System.Linq;
using SuperfastBlur;
using System.Windows.Interop;
using System.Windows;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.Wpf;
using YoutubeExplode;

namespace Equator.Music
{

    internal class GetSong
    {
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);
        public static string VideoId;


#if OFFLINE_IMPLEMENTED
/// <summary>
///     Downloads the first song in the Searchlist response list
/// </summary>
/// <param name="song"></param>
/// <param name="index"></param>
/// <param name="CurrentSong"></param>
/// <returns></returns>
        public static async Task AutoPlaySong(int index, Label CurrentSong, WrapPanel MusicContainer, MediaElement mediaElement, System.Windows.Shapes.Rectangle Background, ChromiumWebBrowser youtubePlayer)
        {
            MusicPanel.SetIndex(index + 1);
            //make it play the first song
            if (MusicPanel.GetIndex() == MusicContainer.Children.Count - 1)
            {
                // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                await
              GetSong.PlaySpecifiedSong(Background, mediaElement,
                  QueryVideo.SearchListResponse.Items[0].Id.VideoId,
                  index,
                  QueryVideo.SearchListResponse.Items[0].Snippet.Title,
                  CurrentSong, youtubePlayer);
            }
            //otherwise play the next song
            await GetSong.PlaySpecifiedSong(Background, mediaElement,
                 QueryVideo.SearchListResponse.Items[index].Id.VideoId,
                 index,
                 QueryVideo.SearchListResponse.Items[index].Snippet.Title,
                 CurrentSong, youtubePlayer);
        }
#endif
        /// <summary>
        ///     Downloads the first song in the Searchlist response list
        /// </summary>
        /// <param name="song"></param>
        /// <param name="index"></param>
        /// <param name="currentSong"></param>
        /// <param name="musicContainer"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static async Task AutoPlaySong(int index, TextBlock currentSong, object musicContainer, System.Windows.Shapes.Rectangle background, ChromiumWebBrowser youtubePlayer, bool playlistPlaying)
        {
            if (!playlistPlaying)
            {
                WrapPanel container = (WrapPanel)musicContainer;
                //make it play the first song if playlist is over only if IsReplay
                if (MusicPanel.GetIndex() == container.Children.Count)
                {
                    if (MusicPanel.IsReplay)
                    {
                        // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                        MusicPanel.SetIndex(0);
                        await
                            PlaySpecifiedSong(background,
                                QueryYoutube.SearchListResponse.Items[0].Id.VideoId,
                                index,
                                QueryYoutube.SearchListResponse.Items[0].Snippet.Title,
                                currentSong, youtubePlayer);
                    }
                    else
                    {
                        currentSong.Text = "No more songs to play!";
                    }
                }
                else
                {
                    //otherwise play the next song
                    await PlaySpecifiedSong(background,
                        QueryYoutube.SearchListResponse.Items[index].Id.VideoId,
                        index,
                        QueryYoutube.SearchListResponse.Items[index].Snippet.Title,
                        currentSong, youtubePlayer);
                }
            }
            else
            {
                StackPanel container = (StackPanel)musicContainer;
                //make it play the first song if playlist is over only if IsReplay
                if (MusicPanel.GetIndex() == container.Children.Count)
                {
                    if (MusicPanel.IsReplay)
                    {
                        // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                        MusicPanel.PlayListIndex = 0;
                        await PlaySpecifiedSong(background,
                            Playlists.CurrentPlaylistItemListResponse.Items[index].Snippet.ResourceId.VideoId,
                            Playlists.CurrentPlaylistItemListResponse.Items[index].Snippet.Title, currentSong, youtubePlayer, Playlists.CurrentPlaylistItemListResponse.Items[index].Snippet.Thumbnails.Medium.Url);

                    }
                    else
                    {
                        currentSong.Text = "Playlist is over!";
                    }
                }
                else
                {

                    //otherwise play the next song
                    await PlaySpecifiedSong(background,
                        Playlists.CurrentPlaylistItemListResponse.Items[index].Snippet.ResourceId.VideoId,
                         Playlists.CurrentPlaylistItemListResponse.Items[index].Snippet.Title, currentSong, youtubePlayer, Playlists.CurrentPlaylistItemListResponse.Items[index].Snippet.Thumbnails.Medium.Url);
                    MusicPanel.PlayListIndex = index;
                }
            }


        }


        public static async Task<string> GetMusicVideo(string videoId, ChromiumWebBrowser youtubePlayer)
        {
            //use video id to get the song
            VideoId = videoId;
#if OFFLINE_IMPLEMENTED
            if (!FilePaths.InCache())
            {
                Console.WriteLine("Video not in cache; video is downloading now");
                return await GetMusic.DownloadVideo(youtubePlayer);
            }
              // Client
            var client = new YoutubeClient();
            var videoInfo = await client.GetVideoInfoAsync(VideoId);
            // Print metadata
           // Console.WriteLine($"Id: {videoInfo.Id} | Title: {videoInfo.Title} | Author: {videoInfo.Author.Title}");
            var fullName = videoInfo.Title;
            //code for the video library api
            var saveName = FilePaths.RemoveIllegalPathCharacters(fullName.Replace("- YouTube", ""));
            saveName = saveName.Replace("_", "");
            string savePath = Path.Combine(FilePaths.SaveLocation(),
                (saveName));

            return savePath;
#endif
            return await DownloadVideo(youtubePlayer);
        }

        /// <summary>
        ///     Gets the song
        /// </summary>
#if OFFLINE_IMPLEMENTED
        public static async Task PlaySpecifiedSong(System.Windows.Shapes.Rectangle backgroundRect, MediaElement mediaElement,
            string musicLink, int index, string songTitle, Label songLabel, ChromiumWebBrowser youtubePlayer)
        {
            songLabel.Content = "Now Playing: " + songTitle;
            MusicPanel.IsPlaying = true;
            MusicPanel.SetIndex(index);
            if (GetMusic.IsConverting)
            {
                GetMusic.FFMpeg.Stop();
            }
            //songLabel.Content = "Loading...";
            Console.WriteLine("Music links: " + musicLink + " " + VideoId);
            //TODO: test for possible issue
            var fullSavePath = await GetMusicVideo(musicLink, youtubePlayer);

            ///<summary>Set the background</summary>
            var fileName = SongThumb.GetSongThumb(
                                QueryVideo.SearchListResponse.Items[MusicPanel.GetIndex()].Snippet.Thumbnails.High.Url,
                         Path.GetFileNameWithoutExtension(fullSavePath));
            var image = System.Drawing.Image.FromFile(fileName);
            var blur = new GaussianBlur(image as Bitmap);
            var blurredThumb = blur.Process(50);
            image.Dispose();
            var hBitmap = blurredThumb.GetHbitmap();
            var backgroundImageBrush = new ImageBrush();
            backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                                                         IntPtr.Zero,
                                                         Int32Rect.Empty,
                                                         BitmapSizeOptions.FromEmptyOptions()
            );
            DeleteObject(hBitmap);
            blurredThumb.Dispose();
            backgroundImageBrush.Stretch = Stretch.UniformToFill;
            backgroundRect.Fill = backgroundImageBrush;
            backgroundRect.Effect = null;
            mediaElement.Opacity = 100;
           

            var saveName = Path.GetFileName(fullSavePath);
            Console.WriteLine("Save name variable in PlaySpecifiedSong method " + saveName);
            try
            {
                var mp4SaveName = saveName.Replace(".webm", ".mp4");

                fullSavePath = Path.Combine(FilePaths.SaveLocation(), mp4SaveName);
                //Console.WriteLine(fullSavePath);
                if (saveName.Contains(".webm") && !File.Exists(fullSavePath))
                {
                    songLabel.Content = "Converting...";
                    await GetMusic.ConvertWebmToMp4(Path.Combine(FilePaths.SaveLocation(),
                        saveName), mp4SaveName);
                    mediaElement.Source = new Uri(fullSavePath);
                }
                else
                {
                    mediaElement.Source = new Uri(fullSavePath);
                }
            }
            catch (NullReferenceException nullReferenceException)
            {
                Console.WriteLine("Some how fullSavePath was not a file path...");
            }


            Console.WriteLine(mediaElement.Source.ToString());
            ///<summary>Renable this when coding in the offline mode</summary>
            ///
            //mediaElement.Play();
        }
#endif
        public static async Task PlaySpecifiedSong(System.Windows.Shapes.Rectangle backgroundRect,
            string musicLink, int index, string songTitle, TextBlock songLabel, ChromiumWebBrowser youtubePlayer)
        {
            songLabel.Text = "Loading...";
            MusicPanel.SetIndex(index);
            var songName = await GetMusicVideo(musicLink, youtubePlayer);
            MusicPanel.IsPlaying = true;
            songLabel.Text = "Now Playing: " + songTitle;
            //Set the background

            var fileName = SongThumb.GetSongThumb(
                QueryYoutube.SearchListResponse.Items[MusicPanel.GetIndex()].Snippet.Thumbnails.High.Url,
                         FilePaths.RemoveIllegalPathCharacters(songName));
            var image = System.Drawing.Image.FromFile(fileName);
            var blur = new GaussianBlur(image as Bitmap);
            Bitmap blurredThumb = null;
            try
            {
                blurredThumb = blur.Process(15);

            }
            catch
            {
                blurredThumb = blur.Process(15);

            }
            image.Dispose();
            var hBitmap = blurredThumb.GetHbitmap();
            var backgroundImageBrush = new ImageBrush();
            backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            DeleteObject(hBitmap);
            blurredThumb.Dispose();
            backgroundImageBrush.Stretch = Stretch.UniformToFill;
            backgroundRect.Fill = backgroundImageBrush;
            backgroundRect.Effect = null;



            /* var saveName = Path.GetFileName(fullSavePath);
             Console.WriteLine("Save name variable in PlaySpecifiedSong method " + saveName);
             try
             {
                 var mp4SaveName = saveName.Replace(".webm", ".mp4");

                 fullSavePath = Path.Combine(FilePaths.SaveLocation(), mp4SaveName);
             }
             catch (NullReferenceException nullReferenceException)
             {
                 Console.WriteLine("Some how fullSavePath was not a file path...");
             }*/

        }
        /// <summary>
        /// Method written for playlists 
        /// </summary>
        /// <param name="backgroundRect"></param>
        /// <param name="musicLink"></param>
        /// <param name="songTitle"></param>
        /// <param name="songLabel"></param>
        /// <param name="youtubePlayer"></param>
        /// <param name="backgroundImageUrl"></param>
        /// <returns></returns>
        public static async Task PlaySpecifiedSong(System.Windows.Shapes.Rectangle backgroundRect,
            string musicLink, string songTitle, TextBlock songLabel, ChromiumWebBrowser youtubePlayer, string backgroundImageUrl)
        {
            songLabel.Text = "Loading...";
            var songName = await GetMusicVideo(musicLink, youtubePlayer);
            MusicPanel.IsPlaying = true;
            songLabel.Text = "Now Playing: " + songTitle;
            //Set the background
            var fileName = SongThumb.GetSongThumb(
                backgroundImageUrl,
                FilePaths.RemoveIllegalPathCharacters(songName));
            var image = System.Drawing.Image.FromFile(fileName);
            var blur = new GaussianBlur(image as Bitmap);
            Bitmap blurredThumb = null;
            try
            {
                blurredThumb = blur.Process(15);

            }
            catch
            {
                blurredThumb = blur.Process(15);

            }
            image.Dispose();
            var hBitmap = blurredThumb.GetHbitmap();
            var backgroundImageBrush = new ImageBrush();
            backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            DeleteObject(hBitmap);
            blurredThumb.Dispose();
            backgroundImageBrush.Stretch = Stretch.UniformToFill;
            backgroundRect.Fill = backgroundImageBrush;
            backgroundRect.Effect = null;
        }

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
