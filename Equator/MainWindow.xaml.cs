using System.Windows;
using Equator.Helpers;

namespace Equator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login1_Click(object sender, RoutedEventArgs e)
        {
            await AuthGoogle.AuthUserCredential();
            var window = new MusicPanel();
            Close();
            window.ShowDialog();
        }
    }
}