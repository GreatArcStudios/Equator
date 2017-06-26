using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Equator.Helpers;
using Path = System.IO.Path;
using System.Drawing;
using SuperfastBlur;
using System.Windows.Interop;
using System.Windows;
using System.Runtime.InteropServices;
using CefSharp.Wpf;
using YoutubeExplode;

namespace Equator.Music
{
   
    internal class GetSong
    {
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);
        public static string VideoId;

        /// <summary>
        ///     Downloads the first song in the Searchlist response list
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
#if OFFLINE_IMPLEMENTED
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
        public static async Task AutoPlaySong(int index, Label CurrentSong, WrapPanel MusicContainer, System.Windows.Shapes.Rectangle Background, ChromiumWebBrowser youtubePlayer)
        {
            //make it play the first song
            if (MusicPanel.GetIndex() == MusicContainer.Children.Count)
            {
                // MusicContainer.Children[GetIndex() + 1].MouseLeftButtonDown
                MusicPanel.SetIndex(0);
                await
              GetSong.PlaySpecifiedSong(Background,
                  QueryVideo.SearchListResponse.Items[0].Id.VideoId,
                  index,
                  QueryVideo.SearchListResponse.Items[0].Snippet.Title,
                  CurrentSong, youtubePlayer);
            }
            else
            {
                //otherwise play the next song
                await GetSong.PlaySpecifiedSong(Background,
                     QueryVideo.SearchListResponse.Items[index].Id.VideoId,
                     index,
                     QueryVideo.SearchListResponse.Items[index].Snippet.Title,
                     CurrentSong, youtubePlayer);
            }
           
        }
            public static async Task<string> GetMusicVideo(string videoId, ChromiumWebBrowser youtubePlayer)
        {
            //use video id to get the song
            VideoId = videoId;
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
            string musicLink, int index, string songTitle, Label songLabel, ChromiumWebBrowser youtubePlayer)
        {
            songLabel.Content = "Now Playing: " + songTitle;
            MusicPanel.IsPlaying = true;
            MusicPanel.SetIndex(index);
            //songLabel.Content = "Loading...";
            Console.WriteLine("Music links: " + musicLink + " " + VideoId);
            //TODO: test for possible issue
            var songName = await GetMusicVideo(musicLink, youtubePlayer);

            ///<summary>Set the background</summary>
            var fileName = SongThumb.GetSongThumb(
                                QueryVideo.SearchListResponse.Items[MusicPanel.GetIndex()].Snippet.Thumbnails.High.Url,
                         FilePaths.RemoveIllegalPathCharacters(songName));
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
    }
}
