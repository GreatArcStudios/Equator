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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Equator.Helpers;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for Music_Cards.xaml
    /// TODO: create logic to update the background of the music panel to match the current song
    /// </summary>
    public partial class MusicCards : UserControl
    {
        public CardHelper CardHelper = new CardHelper();
        public MusicCards(string songTitle, Uri backgroundImageUri)
        {
            InitializeComponent();
            SongTitle.Content = songTitle;
            MusicImage.Source = new BitmapImage(
                CardHelper.GetMusicThumbUri());
           MusicCardContent.MouseEnter += new MouseEventHandler(MusicCard_Content_MouseEnter);
           MusicCardContent.MouseLeave += new MouseEventHandler(MusicCard_Content_MouseLeave);
        }

        private void MusicCard_Content_MouseLeave(object sender, MouseEventArgs e)
        {
            MusicCardContent = (Grid) sender;
            Play.Opacity = 0;
        }

        public void MusicCard_Content_MouseEnter(object sender, MouseEventArgs e)
        {
            MusicCardContent = (Grid)sender;
            Play.Opacity = 100;
        }
    }
}
