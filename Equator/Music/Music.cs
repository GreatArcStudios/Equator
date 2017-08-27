#define USE_YOUTUBEEXPLODE
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CefSharp;
using CefSharp.Wpf;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Image = System.Drawing.Image;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Music
{
    internal class Music
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static string VideoId;


#if OFFLINE_IMPLEMENTED /// <summary>
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
              Music.PlaySpecifiedSong(Background, mediaElement,
                  QueryVideo.SongSearchListResponse.Items[0].Id.VideoId,
                  index,
                  QueryVideo.SongSearchListResponse.Items[0].Snippet.Title,
                  CurrentSong, youtubePlayer);
            }
            //otherwise play the next song
            await Music.PlaySpecifiedSong(Background, mediaElement,
                 QueryVideo.SongSearchListResponse.Items[index].Id.VideoId,
                 index,
                 QueryVideo.SongSearchListResponse.Items[index].Snippet.Title,
                 CurrentSong, youtubePlayer);
        }
#endif
        //public static ArrayList SongThumbUris = new ArrayList();
        public static string GetSongThumb(string url, string songName)
        {
            var filepath = FilePaths.ThumbLocation + "\\" + RemoveIllegalPathCharacters(songName) + ".png";
            try
            {
                var webClient = new WebClient();
                webClient.DownloadFile(url,
                    filepath);
            }
            catch
            {
                return FilePaths.ThumbLocation + "\\" + songName + ".png";
            }

            //SongThumbUris.Add(FilePaths.CreateSaveThumbLocation() + "\\" + songName + ".png");
            return FilePaths.ThumbLocation + "\\" + songName + ".png";
        }
        /// <summary>
        ///     Downloads the first song in the Searchlist response list
        /// </summary>
        /// <param name="song"></param>
        /// <param name="index"></param>
        /// <param name="currentSong"></param>
        /// <param name="musicContainer"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static async Task<bool> AutoPlaySong(int index, TextBlock currentSong,
            Rectangle background, ChromiumWebBrowser youtubePlayer, bool playlistPlaying, Button playButton)
        {
            if (!playlistPlaying)
            {
                //make it play the first song if playlist is over only if IsReplay
                if (MusicPanel.Index == QueryYoutube.PlaylistCount)
                    if (MusicPanel.ReplayState == (int)MusicPanel.ReplayStates.ReplayAll)
                    {
                        // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown

                        var status = await
                             PlaySpecifiedSong(background,
                                 QueryYoutube.SongSearchListResponse.Items[0].Id.VideoId,
                                 index,
                                 QueryYoutube.SongSearchListResponse.Items[0].Snippet.Title,
                                 currentSong, youtubePlayer, playButton);
                        return status;
                    }
                    else
                    {
                        currentSong.Text = "No more songs to play!";
                        MusicPanel.Index = 0;
                        return true;
                    }
                else
                {
                    Console.WriteLine(QueryYoutube.SongSearchListResponse.Items[index].Id.VideoId);
                    var status = await PlaySpecifiedSong(background,
                        QueryYoutube.SongSearchListResponse.Items[index].Id.VideoId,
                        index,
                        QueryYoutube.SongSearchListResponse.Items[index].Snippet.Title,
                        currentSong, youtubePlayer, playButton);
                    return status;
                }

            }
            else
            {

                //make it play the first song if playlist is over only if IsReplay
                if (MusicPanel.PlayListIndex == QueryYoutube.CurrentPlaylistItemListResponse.Items.Count)
                {
                    if (MusicPanel.ReplayState == (int)MusicPanel.ReplayStates.ReplayAll)
                    {
                        // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                        var status = await PlaySpecifiedSong(background,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[index].Snippet.ResourceId.VideoId,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[index].Snippet.Title, currentSong,
                            youtubePlayer,
                            QueryYoutube.CurrentPlaylistItemListResponse.Items[index].Snippet.Thumbnails.Medium.Url, playButton);
                        return status;
                    }
                    else
                    {
                        currentSong.Text = "Playlist is over!";
                        MusicPanel.PlayListIndex = 0;
                        return true;
                    }
                }
                else
                {
                    //otherwise play the next song
                    var status = await PlaySpecifiedSong(background,
                          QueryYoutube.CurrentPlaylistItemListResponse.Items[index].Snippet.ResourceId.VideoId,
                          QueryYoutube.CurrentPlaylistItemListResponse.Items[index].Snippet.Title, currentSong,
                          youtubePlayer,
                          QueryYoutube.CurrentPlaylistItemListResponse.Items[index].Snippet.Thumbnails.Medium.Url, playButton);
                    MusicPanel.PlayListIndex = index;
                    return status;
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
                                QueryVideo.SongSearchListResponse.Items[MusicPanel.GetIndex()].Snippet.Thumbnails.High.Url,
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
        public static string RemoveIllegalPathCharacters(string path)
        {
            //Create a string with all the illegal path + filename chars
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            //Creates a regex with a character group pattern    
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            //Replaces parts of the string that match r's pattern with "" & returns the sanitized str
            return r.Replace(path, "");
        }

        public static async Task<bool> PlaySpecifiedSong(Rectangle backgroundRect,
            string musicLink, int index, string songTitle, TextBlock songLabel, ChromiumWebBrowser youtubePlayer, Button playButton)
        {
            songLabel.Text = "Loading...";
            int maxTries = 4;
            int count = 0;
            while (true)
            {
                try
                {
                    MusicPanel.Index = index;
                    var songName = await GetMusicVideo(musicLink, youtubePlayer);
                    MusicPanel.IsPlaying = true;
                    songLabel.Text = "Now Playing: " + songTitle;
                    var pauseUri = new Uri("Icons/Stop.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(pauseUri);
                    var temp = BitmapFrame.Create(streamInfo.Stream);
                    playButton.Background = new ImageBrush(temp);

                    string fileName;
                    //Set the background
                    if (MusicPanel.PlayingSongs)
                        fileName = GetSongThumb(
                            QueryYoutube.SongSearchListResponse.Items[MusicPanel.Index].Snippet.Thumbnails.High.Url,
                            RemoveIllegalPathCharacters(songName));
                    else
                    {
                        fileName = Music.GetSongThumb(
                            QueryYoutube.SongSearchListResponse.Items[MusicPanel.Index].Snippet.Thumbnails.High.Url,
                            RemoveIllegalPathCharacters(songName));
                    }
                    var image = Image.FromFile(fileName);
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
                    streamInfo.Stream.Close();
                    streamInfo.Stream.Dispose();
                    GC.Collect();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    count++;
                    if (count == maxTries)
                    {
                        return false;
                    }
                }
            }

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
        ///     Method written for playlists
        /// </summary>
        /// <param name="backgroundRect"></param>
        /// <param name="musicLink"></param>
        /// <param name="songTitle"></param>
        /// <param name="songLabel"></param>
        /// <param name="youtubePlayer"></param>
        /// <param name="backgroundImageUrl"></param>
        /// <returns></returns>
        public static async Task<bool> PlaySpecifiedSong(Rectangle backgroundRect,
            string musicLink, string songTitle, TextBlock songLabel, ChromiumWebBrowser youtubePlayer,
            string backgroundImageUrl, Button playButton)
        {
            songLabel.Text = "Loading...";
            int maxTries = 4;
            int count = 0;
            while (true)
            {
                try
                {

                    var songName = await GetMusicVideo(musicLink, youtubePlayer);
                    MusicPanel.IsPlaying = true;
                    songLabel.Text = "Now Playing: " + songTitle;
                    var pauseUri = new Uri("Icons/Stop.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(pauseUri);
                    var temp = BitmapFrame.Create(streamInfo.Stream);
                    playButton.Background = new ImageBrush(temp);
                    //Set the background
                    var fileName = Music.GetSongThumb(
                        backgroundImageUrl,
                        RemoveIllegalPathCharacters(songName));
                    var image = Image.FromFile(fileName);
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
                    streamInfo.Stream.Close();
                    streamInfo.Stream.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    count++;
                    if (count == maxTries)
                    {
                        return false;
                    }
                }

            }
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
            string link = "https://www.youtube.com/watch?v=" + Music.VideoId;
 
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
var uri = "https://www.youtube.com/watch?v=" + Music.VideoId;
var youTube = YouTube.Default;
var video = youTube.GetVideo(uri);*/ //youtubePlayer.Address = "http://33232463.nhd.weebly.com/";
/*var fullName = video.FullName; // same thing as title + fileExtension
var saveName = FilePaths.RemoveIllegalPathCharacters(fullName.Replace("- YouTube", ""));
saveName = saveName.Replace("_", "");
var bytes = await video.GetBytesAsync();
//var stream = video.Stream();*/
#endif
#if USE_YOUTUBEEXPLODE
            // Client
            var client = new YoutubeClient();
            var videoInfo = await client.GetVideoInfoAsync(VideoId);
            // Print metadata
            Console.WriteLine($"Id: {videoInfo.Id} | Title: {videoInfo.Title} | Author: {videoInfo.Author.Title}");

            try
            {
                //var streamInfo = videoInfo.MixedStreams
                //  .OrderBy(s => s.VideoEncoding == VideoEncoding.Vp8)
                // .ThenBy(s => s.VideoQuality == VideoQuality.High720).Last();
                //youtubePlayer.LoadHtml(
                  // $"<html><body scroll=\"no\" style=\"overflow: hidden\"><iframe id = \"youtubePlayer\" src=\"http://youtube.com/embed/{videoInfo.Id}?autoplay=1&controls=0&disablekb=1&enablejsapi=1&rel=0&showinfo=0&showsearch=0\" width=\"480\" height=\"270\" frameborder=\"0\" allowfullscreen=\"\" Style =\"pointer-events:none;\"></iframe>",
                 //  "Music Player");
                youtubePlayer.LoadHtml("<b>fuck this </b>");
                string loadedStr = await youtubePlayer.WebBrowser.GetSourceAsync();
                Console.WriteLine("Player loaded? " + youtubePlayer.IsLoaded + $"| Html loaded: {loadedStr}");
            }
            catch
            {
                //   var streamInfo = videoInfo.MixedStreams
                //        .OrderBy(s => s.VideoEncoding == VideoEncoding.Vp8)
                //      .ThenBy(s => s.VideoQuality == VideoQuality.Medium480).Last();
                youtubePlayer.LoadHtml(
                    $"<html><body scroll=\"no\" style=\"overflow: hidden\"><iframe  id = \"youtubePlayer\" src=\"http://youtube.com/embed/{videoInfo.Id}?autoplay=1&controls=0&disablekb=1&enablejsapi=1&rel=0&showinfo=0&showsearch=0\" width=\"480\" height=\"270\" frameborder=\"0\" allowfullscreen=\"\" Style =\"pointer-events:none;\"></iframe>",
                    "Music Player");
                string loadedStr = await youtubePlayer.WebBrowser.GetSourceAsync();
                Console.WriteLine("Player loaded? " + youtubePlayer.IsLoaded + $"| Html loaded: {loadedStr}");
            }

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

#if OFFLINE_IMPLEMENTED
        public static bool InCache()
        {
            var uri = "https://www.youtube.com/watch?v=" + Music.VideoId;
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