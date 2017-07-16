using System.Windows;
using Equator.Helpers;
using MahApps.Metro.Controls;
using System;
using SuperfastBlur;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login1_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.GetFiles(FilePaths.SaveUserCreds()).Length == 0)
            {
                await GoogleServices.AuthUserCredential();
            }
            var window = new MusicPanel();
            window.Show();
            Close();
        }
    }
}