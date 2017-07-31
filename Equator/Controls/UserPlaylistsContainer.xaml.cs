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

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for UserPlaylistsContainer.xaml
    /// </summary>
    public partial class UserPlaylistsContainer : UserControl
    {
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);
        public UserPlaylistsContainer()
        {
            InitializeComponent();
            var image = new BitmapImage(new Uri(FilePaths.SaveUserImage()));
            var bitmap = convertBitmap(image);
            var blur = new GaussianBlur(bitmap);
            Bitmap blurredThumb = null;
            try
            {
                blurredThumb = blur.Process(15);

            }
            catch
            {
                blurredThumb = blur.Process(15);

            }
            bitmap.Dispose();
            var hBitmap = blurredThumb.GetHbitmap();
            var backgroundImageBrush = new ImageBrush();
            backgroundImageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            DeleteObject(hBitmap);
            blurredThumb.Dispose();
            backgroundImageBrush.Stretch = Stretch.UniformToFill;
            BackgroundImage.Source = backgroundImageBrush.ImageSource;
        }
        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/13147707-a9d3-40b9-82e4-290d1c64ccac/bitmapbitmapimage-conversion?forum=wpf
        /// </summary>
        /// <param name="bitImage"></param>
        /// <returns></returns>
        private Bitmap convertBitmap(BitmapImage bitImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return bitmap;
            }
        }
        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => { Container.Opacity = 0; });
        }
    }
}
