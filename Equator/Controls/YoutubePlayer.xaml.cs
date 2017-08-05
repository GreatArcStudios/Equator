using System;
using System.Windows.Controls;
using CefSharp;

namespace Equator.Controls
{
    /// <summary>
    ///     Interaction logic for YoutubePlayer.xaml
    /// </summary>
    public partial class YoutubePlayer : UserControl
    {
        internal bool Minimized = false;

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
    }
}
