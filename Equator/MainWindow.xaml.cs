using System.Windows;
using Equator.Helpers;
using MahApps.Metro.Controls;

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
            await GoogleServices.AuthUserCredential();
            var window = new MusicPanel();
            Close();
            window.ShowDialog();
        }
    }
}