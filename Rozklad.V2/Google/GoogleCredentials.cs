namespace Rozklad.V2.Google
{
    public class GoogleCredentials
    {
        public GoogleCredentials(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
        public readonly string ClientId;

        public readonly string ClientSecret;
    }
}