using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Equator.Helpers
{
    class SongThumb
    {
        public static string GetSongThumb(string url, string songName)
        {
             WebClient webClient = new WebClient();
             webClient.DownloadFile(url, FilePaths.saveThumb()+"\\"+songName+".png" );
            return FilePaths.saveThumb() + "\\"+ songName + ".png";
        }   
    }
}
