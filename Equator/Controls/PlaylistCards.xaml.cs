using System;
using System.Collections.Generic;
using System.IO;
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
using CefSharp.Wpf;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Controls
{
    /// <summary>
    /// Interaction logic for PlaylistCards.xaml
    /// </summary>
    public partial class PlaylistCards : UserControl
    {
        internal PlaylistItemListResponse _playlistItemListResponse;
        public PlaylistCards(bool userPlaylist, string playlistName, PlaylistItemListResponse playlistItemListResponse, TextBlock songLabel, Rectangle backgroundRectangle, ChromiumWebBrowser youtubePlayer)
        {
            InitializeComponent();
            _playlistItemListResponse = playlistItemListResponse;
            Console.WriteLine(playlistName + " has " + playlistItemListResponse.Items.Count);
            if (userPlaylist)
            {
                //create user image
                ImageBrush userImageBrush;
                if (!File.Exists(FilePaths.SaveUserImage() + "\\Userimage.png"))
                    userImageBrush = new ImageBrush(new BitmapImage(new Uri(GoogleServices.GetUserPicture())));
                else
                {
                    userImageBrush =
                        new ImageBrush(new BitmapImage(new Uri(FilePaths.SaveUserImage() + "\\Userimage.png")));
                }
                userImageBrush.TileMode = TileMode.None;
                UserPlaylistCover.Fill = userImageBrush;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Uri backgroundImageUri = new Uri(_playlistItemListResponse.Items[i].Snippet.Thumbnails.Medium.Url);
                    ImageSource tempSource = new BitmapImage(backgroundImageUri);
                    ((Image) SearchedPlaylistImagesCover.Children[i]).Source = tempSource;
                }
            }
            Channel_name.Content = playlistName;

        }
    }
}
