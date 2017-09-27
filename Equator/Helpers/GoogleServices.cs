using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Equator.Properties;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace Equator.Helpers
{
    internal class GoogleServices
    {
        public static string ApiKey = "AIzaSyCFb8JKlmaP95MqqwYYBDoqvsy7YwRxztM";
        public static UserCredential Credential;
        public static YouTubeService YoutubeService;
        public static Person UserPerson; 
        private static PlusService _plusService;

        public static YouTubeService CreateYoutubeService(string apiKey, bool oAuth2, UserCredential credential)
        {
            if (oAuth2)
                YoutubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    HttpClientInitializer = credential,
                    ApplicationName = "EQUATORPLAYER"
                });
            else
                YoutubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    ApplicationName = "EQUATORPLAYER"
                });

            return YoutubeService;
        }

        public static PlusService CreatePlusService(string apiKey, bool oAuth2, UserCredential credential)
        {
            if (oAuth2)
                _plusService = new PlusService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "EQUATORPLAYER"
                });
            else
                _plusService = new PlusService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    ApplicationName = "EQUATORPLAYER"
                });

            return _plusService;
        }

        public static async Task AuthUserCredential(bool GetPicture)
        {
            Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "440102560893-e959uso94cdvtea9kou6e5uc0a8qkfro.apps.googleusercontent.com",
                    ClientSecret = "7sWMyxs0qODeTstnJx8CAVti"
                },
                // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                // user's account, but not other types of account access.
                new[] {YouTubeService.Scope.Youtube, PlusService.Scope.PlusMe},
                "user",
                CancellationToken.None,
                new FileDataStore(FilePaths.UserCredLocation, true)
            );

            if(GetPicture)
            GetUserPicture();
            else
            {
                UserPerson = CreatePlusService(ApiKey, true, Credential).People.Get("me").Execute();
            }
            Console.WriteLine("Authenticated");
        }

        public static string GetUserPicture()
        {
            var userPerson = CreatePlusService(ApiKey, true, Credential).People.Get("me").Execute();
            try
            {
                var userImageUrl = userPerson.Image.Url;
                userImageUrl = userImageUrl.Replace("sz=50", "sz=1000");
                var webClient = new WebClient();
                webClient.DownloadFile(userImageUrl,
                    FilePaths.UserImageLocation + "\\" + "Userimage.png");
            }
            catch
            {
                Console.WriteLine("Picture in use");
            }
            return FilePaths.UserImageLocation + "\\" + "Userimage.png";
        }

        public static async void LogOut()
        {
            await Credential.RevokeTokenAsync(CancellationToken.None);
        }
    }
}