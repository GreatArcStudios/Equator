using System.Windows;
using System.Windows.Controls;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for UserPlaylistsContainer.xaml
    /// </summary>
    public partial class UserPlaylistsContainer : UserControl
    {
        private readonly PlaylistCards _card;

        public UserPlaylistsContainer(PlaylistCards card)
        {
            InitializeComponent();
            _card = card;
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Container.Opacity = 0;
                Panel.SetZIndex(_card, 3);
            });
        }
    }
}