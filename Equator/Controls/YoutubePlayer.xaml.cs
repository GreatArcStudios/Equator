using System.Windows.Controls;
using System.Windows.Media.Animation;
using Equator.Helpers;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for YoutubePlayer.xaml
    /// </summary>
    public partial class YoutubePlayer : UserControl
    {
        private bool _minimized = false;
        public YoutubePlayer()
        {
            InitializeComponent();

        }

        private void Minimize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SmoothTransition smoothTransition = new SmoothTransition(0.03);
            if (!_minimized)
            {
                ((Storyboard)FindResource("minimize")).Begin(Container);
                _minimized = true;
            }
            else
            {
                ((Storyboard)FindResource("fadein")).Begin(Minimize);
                ((Storyboard)FindResource("maximize")).Begin(Container);
                _minimized = false;
            }

        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(!_minimized)
            ((Storyboard)FindResource("fadein")).Begin(Minimize);

        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(!_minimized)
            ((Storyboard)FindResource("fadeout")).Begin(Minimize);
        }
    }
}
