using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace Equator.Helpers
{
    internal class GoogleServices
    {
        public static string ApiKey = "AIzaSyCFb8JKlmaP95MqqwYYBDoqvsy7YwRxztM";
        public static UserCredential Credential;
        private static YouTubeService _youtubeService;
        private static PlusService _plusService;

        public static YouTubeService CreateYoutubeService(string apiKey, bool oAuth2, UserCredential credential)
        {
            if (oAuth2)
                _youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "EQUATORPLAYER"
                });
            else
                _youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    ApplicationName = "EQUATORPLAYER"
                });

            return _youtubeService;
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

        public static async Task AuthUserCredential()
        {
            Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "440102560893-e959uso94cdvtea9kou6e5uc0a8qkfro.apps.googleusercontent.com",
                    ClientSecret = "7sWMyxs0qODeTstnJx8CAVti"
                },
                // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                // user's account, but not other types of account access.
                new[] { YouTubeService.Scope.Youtube, PlusService.Scope.PlusMe },
                "user",
                CancellationToken.None,
                new FileDataStore(FilePaths.SaveUserCreds(), true)
            );
            GetUserPicture();
            Console.WriteLine("Authenticated");
        }

        public static string GetUserPicture()
        {
           
            var userPerson = CreatePlusService(ApiKey, true, Credential).People.Get("me").Execute();
            try
            {
                var webClient = new WebClient();
                webClient.DownloadFile(userPerson.Image.Url,
                    FilePaths.SaveUserImage() + "\\" + "Userimage.png");
            }
            catch
            {
                Console.WriteLine("Picture in use");
            }
            return FilePaths.SaveUserImage() + "\\" + "Userimage.png";
        }

        public static async void LogOut()
        {
            await Credential.RevokeTokenAsync(CancellationToken.None);
        }
    }
}