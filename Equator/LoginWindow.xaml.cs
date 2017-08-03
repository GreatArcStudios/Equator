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
            if (Equator.Properties.Settings.Default.FirstRun)
            {
                SaveLocation();
                SaveThumb();
                SaveUserImage();
                SaveUserCreds();
                Equator.Properties.Settings.Default.FirstRun = false;
                Settings.Default.Save();
            }

            if (Directory.GetFiles(FilePaths.UserCredLocation).Length == 0)
            {
                File.Delete(FilePaths.UserImageLocation + "\\Userimage.png");
                await GoogleServices.AuthUserCredential();
            }
            var window = new MusicPanel();
            window.Show();
            Close();
        }
        public string SaveLocation()
        {
            if (!Directory.Exists(FilePaths.saveLocation))
                Directory.CreateDirectory(FilePaths.saveLocation);
            return FilePaths.saveLocation;
        }

        public string SaveThumb()
        {
            if (!Directory.Exists(FilePaths.ThumbLocation))
                Directory.CreateDirectory(FilePaths.ThumbLocation);
            return FilePaths.ThumbLocation;
        }

        public static string SaveUserImage()
        {
            if (!Directory.Exists(FilePaths.UserImageLocation))
                Directory.CreateDirectory(FilePaths.UserImageLocation);
            return FilePaths.UserImageLocation;
        }

        public static string SaveUserCreds()
        {
            if (!Directory.Exists(FilePaths.UserCredLocation))
                Directory.CreateDirectory(FilePaths.UserCredLocation);
            return FilePaths.UserCredLocation;
        }

    }
}