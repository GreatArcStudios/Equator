#define DEBUG
using System;
using System.IO;
using System.Windows;
using Equator.Helpers;
using MahApps.Metro.Controls;
using Equator.Properties;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login1_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (Equator.Properties.Settings.Default.FirstRun)
            {
                CreateSaveLocation();
                CreateSaveThumbLocation();
                CreateSaveUserImageLocation();
                CreateSaveUserCredsLocation();
                Equator.Properties.Settings.Default.FirstRun = false;
                Settings.Default.Save();
            }*/
#if DEBUG
            CreateSaveLocation();
            CreateSaveThumbLocation();
            CreateSaveUserImageLocation();
            CreateSaveUserCredsLocation();
            Equator.Properties.Settings.Default.FirstRun = false;
            Settings.Default.Save();
#endif


            if (Directory.GetFiles(FilePaths.UserCredLocation).Length == 0)
            {
                File.Delete(FilePaths.UserImageLocation + "\\Userimage.png");
                await GoogleServices.AuthUserCredential(true);
            }
            else
                await GoogleServices.AuthUserCredential(false);
            var window = new MusicPanel();
            GoogleServices.YoutubeService =
                GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, true, GoogleServices.Credential);
            window.Show();
            Close();
        }
        public string CreateSaveLocation()
        {
            if (!Directory.Exists(FilePaths.SaveLocation))
                Directory.CreateDirectory(FilePaths.SaveLocation);
            return FilePaths.SaveLocation;
        }

        public string CreateSaveThumbLocation()
        {
            if (!Directory.Exists(FilePaths.ThumbLocation))
                Directory.CreateDirectory(FilePaths.ThumbLocation);
            return FilePaths.ThumbLocation;
        }

        public static string CreateSaveUserImageLocation()
        {
            if (!Directory.Exists(FilePaths.UserImageLocation))
                Directory.CreateDirectory(FilePaths.UserImageLocation);
            return FilePaths.UserImageLocation;
        }

        public static string CreateSaveUserCredsLocation()
        {
            if (!Directory.Exists(FilePaths.UserCredLocation))
                Directory.CreateDirectory(FilePaths.UserCredLocation);
            return FilePaths.UserCredLocation;
        }

    }
}