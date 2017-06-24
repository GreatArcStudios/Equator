using System.Windows.Controls;
using Equator.Helpers;
using WMPLib;
using CefSharp.Wpf;
using CefSharp;

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

        public static void PlayVideo(ChromiumWebBrowser youtubePlayer)
        {
            string script = "(function(){var youtubePlayer = document.getElementById('youtubePlayer')})();";
            youtubePlayer.ExecuteScriptAsync(script);
        }

        public static void PauseVideo(ChromiumWebBrowser youtubePlayer)
        {
            string script = "var youtubePlayer = document.getElementById(\"youtubePlayer\");";
            script += "youtubePlayer.pause();";
            youtubePlayer.ExecuteScriptAsync(script);
        }

        //add in parm for video url 
        public static void ReplayVideo()
        {
            //Player.URL = FilePaths.SaveLocation() + GetMusic.SongTitle;
            //Player.controls.play();
        }

        public static void Shuffle()
        {
        }
    }
}