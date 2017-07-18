using System;
using System.Threading.Tasks;
using Equator.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Equator.Music
{
    /*
    Gets various playlists off of youtube
    */

    internal class PredefinedPlaylists
    {
        /**  NOT DONE TOPIC PLAYIST ISSUES 
        public static void GetTopicPlayist()
        {
            string channelId = QueryYoutube.searchListResponse.Items[0].Id.ChannelId;

        }  **/
        public static async Task<PlaylistListResponse> GetUserPlaylist()
        {
            await GoogleServices.AuthUserCredential();  
            var credential = GoogleServices.Credential;
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, true, credential);
            var userPlaylistRequest = service.Playlists.List("snippet");
            userPlaylistRequest.Mine = true;
            var userPlaylistResponse = await userPlaylistRequest.ExecuteAsync();
            Console.WriteLine("First id response from getuserplaylist " + userPlaylistResponse.Items[0].Id);
            return userPlaylistResponse;
        }
        
        /*
           Get top 50 songs from this playlist: https://www.youtube.com/playlist?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG
        */
        public static PlaylistItemListResponse GetTopSongs()
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;
        }

        public static PlaylistItemListResponse GetNewSongs()
        {
            var service = GoogleServices.CreateYoutubeService(GoogleServices.ApiKey, false, null);
            var topPlaylistItemRequest = service.PlaylistItems.List("snippet");
            topPlaylistItemRequest.PlaylistId = "PLvFYFNbi-IBFeP5ALr50hoOmKiYRMvzUq";
            var playlistItemResponse = topPlaylistItemRequest.Execute();
            return playlistItemResponse;
        }
    }
}