﻿using System.Windows;
using System.Windows.Controls;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for VolumePanel.xaml
    /// </summary>
    public partial class VolumePanel : UserControl
    {
        public double Volume = 100.00;

        //private MediaElement _mediaElement;
        public VolumePanel()
        {
            InitializeComponent();
            Slider.Maximum = 1;
            Slider.Minimum = 0;
        }

        /* private void VolumeBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            slider = (Slider)sender;
        }
        private void VolumeBar_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            slider = (Slider)sender;
            Volume = slider.Value;
            label.Content = slider.Value + "%";
            //_mediaElement.Volume = Volume;
            _youtubePlayer.GetMainFrame().ExecuteJavaScriptAsync(String.Format("(function(){youtubePlayer.volume = {0}})();",Volume));
        }
        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }*/
        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(() =>
            {
                Volume = Slider.Value;
                var labelContent = (int) (Volume * 100);
                Label.Content = labelContent + "%";
            });
        }
    }
}