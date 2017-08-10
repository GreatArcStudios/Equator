using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for UserPlaylistsContainer.xaml
    /// </summary>
    public partial class UserPlaylistsContainer : UserControl
    {
        private readonly PlaylistCards _card;
        private readonly ScrollViewer _playlistScrollViewer;
        public UserPlaylistsContainer(PlaylistCards card, ScrollViewer playlistScrollViewer)
        {
            InitializeComponent();
            _card = card;
            _playlistScrollViewer = playlistScrollViewer;
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                this.Opacity = 0;
                Panel.SetZIndex(this, -9999);
                IsEnabled = false;
                Panel.SetZIndex(_playlistScrollViewer, 3);
                _playlistScrollViewer.Opacity = 100;
            });
        }
    }
}