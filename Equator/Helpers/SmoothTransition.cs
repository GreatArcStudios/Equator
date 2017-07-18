using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Equator.Helpers
{
    class SmoothTransition
    {
        private FrameworkElement _frameworkElement;
        private double _targetWidth;
        private double _targetHeight;
        private bool _isGrowing = false;
        private Timer _timer = new Timer();
        /// <summary>
        /// interval is time it takes in seconds - should be very quick
        /// </summary>
        /// <param name="interval"></param>
        public SmoothTransition(double interval)
        {
            _timer.Interval = interval * 1000;
            _timer.Elapsed += new ElapsedEventHandler(tick);
        }
        /// <summary>
        /// Calculate the resize per tick based on delta to target size and fixed removal size 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void tick(object sender, EventArgs eventArgs)
        {
            double dWidth = 0;
            double dHeight = 0;
            Application.Current.Dispatcher.Invoke(() => {
               dWidth = Math.Abs(_targetWidth - _frameworkElement.Width);
               dHeight = Math.Abs(_targetHeight - _frameworkElement.Height);
            });
            Console.Write(dWidth + " "  + dHeight);
            if (dHeight < 1 && dWidth < 1)
            {
                _timer.Stop();
                //_timer.Dispose();
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!_isGrowing)
                {
                    if (dWidth - 10 < 1 && dHeight - 10 > 1)
                    {
                        _frameworkElement.Height -= 10;
                        _frameworkElement.Width = 0;
                    }
                    else if (dHeight - 10 < 1 && dWidth - 10 > 1)
                    {
                        _frameworkElement.Width -= 10;
                        _frameworkElement.Height -= 0;
                    }

                    else
                    {
                        if (dWidth > dHeight)
                        {
                            _frameworkElement.Width -= 10;
                        }
                        else
                        {
                            _frameworkElement.Height -= 10;
                            _frameworkElement.Width -= 10;
                        }

                    }
                }
                else
                {
                    if (dWidth + 10 < 1 && dHeight + 10 > 1)
                    {
                        _frameworkElement.Height += 10;
                        _frameworkElement.Width = _targetWidth;
                    }
                    else if (dHeight + 10 < 1 && dWidth + 10 > 1)
                    {
                        _frameworkElement.Width += 10;
                        _frameworkElement.Height += _targetHeight;
                    }

                    else
                    {
                        if (dWidth > dHeight)
                        {
                            _frameworkElement.Width += 10;
                        }
                        else
                        {
                            _frameworkElement.Height += 10;
                            _frameworkElement.Width += 10;
                        }

                    }
                }
                
                Console.WriteLine("Resized: " + _frameworkElement.Width + _frameworkElement.Height);
            });
        }
        /// <summary>
        /// frameworkElement is the target UI element
        /// targetWidth is the width you want to scale it to 
        /// targetHeight is the height you want to scale it to
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        internal void Resize(FrameworkElement frameworkElement, double targetWidth, double targetHeight, bool isGrowing)
        {
            _targetHeight = targetHeight;
            _targetWidth = targetWidth;
            _frameworkElement = frameworkElement;
            _isGrowing = isGrowing;
            _timer.Enabled = true;
            _timer.Start();

        }
    }
}
