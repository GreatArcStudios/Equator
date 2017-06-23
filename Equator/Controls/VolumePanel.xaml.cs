using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for VolumePanel.xaml
    /// </summary>
    public partial class VolumePanel : UserControl
    {
        public static double Volume = 100.00;
        private MediaElement _mediaElement; 
        public VolumePanel(MediaElement mediaElement)
        {
            InitializeComponent();
            slider.Value = mediaElement.Volume;
            Volume = mediaElement.Volume;
            _mediaElement = mediaElement;
        }
        private void VolumeBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            slider = (Slider)sender;
        }
        private void VolumeBar_DragEnded(object sender, DragStartedEventArgs e)
        {
            slider = (Slider)sender;
            Volume = slider.Value;
            label.Content = slider.Value.ToString();
            _mediaElement.Volume = Volume;
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
