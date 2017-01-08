using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Equator.Helpers;
using Equator.Music;
using MahApps.Metro.Controls;
using VideoLibrary;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MusicPanel.xaml
    /// </summary>
    public partial class MusicPanel : MetroWindow
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
        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Show_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowLeftMenu", Hide_Button, Show_Button, Nav_Panel);
        }

        private void Hide_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideLeftMenu", Hide_Button, Show_Button, Nav_Panel);
        }

        private void User_login_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}