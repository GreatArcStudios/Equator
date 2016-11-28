using System.Windows.Controls;
using Equator.Helpers;
using WMPLib;

namespace Equator.Music
{
    internal class MusicControls
    {
        public static WindowsMediaPlayer player = new WindowsMediaPlayer();

        public static void LoadSong()
        {
            player.URL = FilePaths.SaveLocation() + GetMusic.SongTitle;
        }

        public static void PlaySong()
        {
            player.controls.play();
        }

        public static void PauseSong()
        {
            player.controls.pause();
        }

        public static void PlayVideo(MediaElement musicPlayer)
        {
            musicPlayer.Play();
        }

        public static void PauseVideo(MediaElement musicPlayer)
        {
            musicPlayer.Pause();
            ;
        }
    }
}