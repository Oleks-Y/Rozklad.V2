using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Requests;
using Newtonsoft.Json;
using Rozklad.V2.Exceptions;
using Rozklad.V2.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rozklad.V2.Google
{
    public class GoogleTokenService
    {
        private readonly GoogleCredentials _credentials;

        public GoogleTokenService(GoogleCredentials creads)
        {
            _credentials = creads;
        }
        
        ///
        /// we need accessToken for requests to user`s google calendar
        /// so every time, when we need it, we request fresh accessToken from google 
        ///
        public async Task<string> GetActualAccessToken(string refreshToken)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            string requestString = $"http://oauth2.googleapis.com/token?" +
                                   $"client_secret={_credentials.ClientSecret}" +
                                   $"&grant_type=refresh_token" +
                                   $"&refresh_token={refreshToken}" +
                                   $"&client_id={_credentials.ClientId}";
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestString))
            {
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                response = await client.SendAsync(request);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException(
                    $"wrong response code was returned from method POST token/ : {response.StatusCode}\n {await response.Content.ReadAsStringAsync()}");
            }

            var responseToken =
                JsonSerializer.Deserialize<RefreshTokenResponse>(await response.Content.ReadAsStringAsync());

            return responseToken.AccessToken;
        }
    }
}