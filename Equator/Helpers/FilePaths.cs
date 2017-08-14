using System;
using System.IO;
using System.Text.RegularExpressions;

//using VideoLibrary;

namespace Equator.Helpers
{
    internal static class FilePaths
    {
        public static readonly string DefaultImageLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\userdata\\images\\DefaultImage.png";

        public static readonly string SaveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                      "\\Equator Music\\cache";

        public static readonly string ThumbLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\cache\\images";

        public static readonly string UserImageLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\userdata\\images";

        public static readonly string UserCredLocation =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            "\\Equator Music\\userdata\\credentials";

     
            
       
    }
}