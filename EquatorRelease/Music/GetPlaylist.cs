using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equator.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Music
{
    /*
    Gets various playlists off of youtube
    */
    class GetPlaylist
    {
        /**  NOT DONE TOPIC PLAYIST ISSUES 
        public static void GetTopicPlayist()
        {
            string channelId = QueryVideo.searchListResponse.Items[0].Id.ChannelId;

        }  **/

        public static async Task<PlaylistListResponse> ListPlaylist()
        { 
            UserCredential credential = AuthGoogle.credential;
            var service = AuthGoogle.CreateService(AuthGoogle.ApiKey, true, credential);
            var userPlaylistRequest = service.Playlists.List("snippet");
            userPlaylistRequest.Mine = true;
            var userPlaylistResponse = userPlaylistRequest.Execute();
            return userPlaylistResponse;
        }
        /*
           Get top 50 songs from this playlist: https://www.youtube.com/playlist?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG
        */
        public static PlaylistItemListResponse GetTopSongs ()
        {
           var service =  AuthGoogle.CreateService(AuthGoogle.ApiKey, false, null);
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;

        }
    }
}
