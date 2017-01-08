using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Equator.Music;

namespace Equator
{
    /// <summary>
    /// Interaction logic for MusicPanel.xaml
    /// </summary>
    public partial class MusicPanel : Window
    {
        public MusicPanel()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            GetSong.GetMusic(Searchfield.Text); 
        }
    }
}
