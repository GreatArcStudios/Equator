using System;
using System.Windows.Controls;
using Equator.Helpers;
using WMPLib;

namespace Equator.Music
{
    internal class MusicControls
    {
        public static WindowsMediaPlayer Player = new WindowsMediaPlayer();

        public static void LoadSong()
        {
            Player.URL = FilePaths.SaveLocation() + GetMusic.SongTitle;
        }

        public static void PlaySong()
        {
            Player.controls.play();
        }

        public static void PauseSong()
        {
            Player.controls.pause();
        }

        public static void PlayVideo(MediaElement musicPlayer)
        {
            musicPlayer.Play();       
            
        }

        public static void PauseVideo(MediaElement musicPlayer)
        {
            musicPlayer.Pause();
           
        }
        //add in parm for video url 
        public static void ReplayVideo()
        {
            Player.URL = FilePaths.SaveLocation() + GetMusic.SongTitle;
            Player.controls.play();
        }

        public static void Shuffle()
        {
            
            
        }
    }
}