using System;
using System.IO;
using System.Windows;
using Equator.Helpers;
using Equator.Music;
using VideoLibrary;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MusicPanel.xaml
    /// </summary>
    public partial class MusicPanel : Window
    {
        public MusicPanel()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await GetSong.GetMusic(Searchfield.Text);
            if (!FilePaths.InCache())
            {
                VideoPlayer.Source = new Uri(Path.Combine(FilePaths.SaveLocation(),
                    FilePaths.RemoveIllegalPathCharacters(GetMusic.SongTitle)));
            }
            else
            {
                var uri = "https://www.youtube.com/watch?v=" + GetSong.VideoID;
                var youTube = YouTube.Default;
                var video = youTube.GetVideo(uri);
                var fullName = video.FullName;
                var saveName = fullName.Replace("- YouTube", "");
                if (saveName.Contains(".webm"))
                {
                    var mp4SaveName = saveName.Replace(".webm", ".mp4");
                    await GetMusic.ConvertWebmToMp4(Path.Combine(FilePaths.SaveLocation(),
                        FilePaths.RemoveIllegalPathCharacters(saveName)), mp4SaveName);
                    VideoPlayer.Source = new Uri(Path.Combine(FilePaths.SaveLocation(),
                        FilePaths.RemoveIllegalPathCharacters(mp4SaveName)));
                }
                else
                {
                    VideoPlayer.Source = new Uri(Path.Combine(FilePaths.SaveLocation(),
                        FilePaths.RemoveIllegalPathCharacters(saveName)));
                }
            }

            Console.WriteLine(VideoPlayer.Source.ToString());
            VideoPlayer.Play();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MusicControls.PauseVideo(VideoPlayer);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MusicControls.PlayVideo(VideoPlayer);
        }
    }
}