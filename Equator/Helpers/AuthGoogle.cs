using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace Equator.Helpers
{
    internal class AuthGoogle
    {
        public static string ApiKey = "AIzaSyCFb8JKlmaP95MqqwYYBDoqvsy7YwRxztM";
        public static UserCredential credential;
        private static YouTubeService youtubeService;

        public static YouTubeService CreateService(string apiKey, bool OAuth2, UserCredential credential)
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
                new[] {YouTubeService.Scope.Youtube},
                "user",
                CancellationToken.None,
                new FileDataStore(FilePaths.SaveLocation(), true)
            );
            Console.WriteLine("Authenticated");
        }
    }
}