using System.Windows.Controls;
using Equator.Helpers;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for YoutubePlayer.xaml
    /// </summary>
    public partial class YoutubePlayer : UserControl
    {
        private bool minimized = false;
        public YoutubePlayer()
        {
            InitializeComponent();
         
        }

        private void Minimize_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            SmoothTransition smoothTransition = new SmoothTransition(0.03);
            if (!minimized)
            {
                smoothTransition.Resize(CefPlayer, 0, 0, false);
                minimized = true;
            }
            else
            {
                smoothTransition.Resize(CefPlayer, 480, 270, true);
                minimized = false;
            }
            
        }
    }
}
