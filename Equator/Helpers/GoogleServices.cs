using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        public static UserCredential credential;
        private static YouTubeService youtubeService;
        private static PlusService plusService;
        public static YouTubeService CreateYoutubeService(string apiKey, bool OAuth2, UserCredential credential)
        {
            if (OAuth2)
                youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "EQUATORPLAYER"
                });
            else
                youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    ApplicationName = "EQUATORPLAYER"
                });

            return youtubeService;
        }

        public static PlusService CreatePlusService(string apiKey, bool OAuth2, UserCredential credential)
        {
            if (OAuth2)
            {
                plusService= new PlusService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "EQUATORPLAYER",
                });
            }
            else
            {
                plusService = new PlusService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = "EQUATORPLAYER",
                });
            }
           
            return plusService;
        }
        public static async Task AuthUserCredential()
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "440102560893-e959uso94cdvtea9kou6e5uc0a8qkfro.apps.googleusercontent.com",
                    ClientSecret = "7sWMyxs0qODeTstnJx8CAVti"
                },
                // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                // user's account, but not other types of account access.
                new[] {YouTubeService.Scope.Youtube, PlusService.Scope.UserinfoProfile},
                "user",
                CancellationToken.None,
                new FileDataStore(FilePaths.SaveLocation(), true)
            );
            Console.WriteLine("Authenticated");
        }

        public string GetUserPicture()
        {
            Person userPerson = CreatePlusService(ApiKey, true, credential).People.Get("me").Execute();
            var webClient = new WebClient();
            webClient.DownloadFile(userPerson.Image.Url,
                FilePaths.saveUserImage() + "\\" + "Userimage.png");
            return FilePaths.saveUserImage() + "\\" + "Userimage.png";
        }
    }
}