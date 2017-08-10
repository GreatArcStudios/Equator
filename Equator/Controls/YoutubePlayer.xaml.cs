using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CefSharp;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for YoutubePlayer.xaml
    /// </summary>
    public partial class YoutubePlayer : UserControl
    {
        internal int State = 0;
        internal enum WindowStates 
        {
            Normal,
            Minimized,
            Maximized
        }

        public YoutubePlayer()
        {
            InitializeComponent();
            CefPlayer.MenuHandler = new MenuHandler();       
        }
        private class MenuHandler : IContextMenuHandler
        {
            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {
                
            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }

            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
            }

            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                return false;
            }
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Storyboard)FindResource("Fadeinoverlay")).Begin(YoutubePlayerOverlay);
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Storyboard)FindResource("Fadeoutoverlay")).Begin(YoutubePlayerOverlay);
        }
    }
}
