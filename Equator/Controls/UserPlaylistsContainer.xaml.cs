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

        public UserPlaylistsContainer(PlaylistCards card)
        {
            InitializeComponent();
            _card = card;
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                this.Opacity = 0;
                Panel.SetZIndex(((WrapPanel)VisualTreeHelper.GetParent(_card)), 3);
                ((WrapPanel) VisualTreeHelper.GetParent(_card)).Opacity = 100;
                Panel.SetZIndex(this, -9999);
                IsEnabled = false;
            });
        }
    }
}