using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equator.Helpers
{
    class FilePaths
    {
        private static string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Equator Music\\cache" ;

        private static string thumbLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                              "\\Equator Music\\cache\\images";
        public static string SaveLocation () {
            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }
            return saveLocation;
        }

        public static string saveThumb()
        {
            if (!Directory.Exists(thumbLocation))
            {
                Directory.CreateDirectory(thumbLocation);
            }
            return thumbLocation;
        }
    }
}
