using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Equator.Helpers;
using SuperfastBlur;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for UserPlaylistsContainer.xaml
    /// </summary>
    public partial class UserPlaylistsContainer : UserControl
    {
        private PlaylistCards _card;
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
