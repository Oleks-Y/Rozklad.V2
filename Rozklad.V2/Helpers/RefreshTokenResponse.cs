using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Rozklad.V2.Helpers
{
    /// <summary>
    /// Body of response for token request 
    /// </summary>
    public class RefreshTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}