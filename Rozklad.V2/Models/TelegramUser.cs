using System.Reflection.PortableExecutable;
using System.Text.Json.Serialization;

namespace Rozklad.V2.Models
{
    public class TelegramUser
    {
        [JsonPropertyName("auth_date")]
        public long auth_date { get; set; }
        [JsonPropertyName("first_name")]
        public string first_name { get; set; }
        [JsonPropertyName("hash")]
        public string hash { get; set; }
        [JsonPropertyName("id")]
        public long id { get; set; }
        [JsonPropertyName("last_name")]
        public string last_name { get; set; }
        [JsonPropertyName("photo_url")]
        public string photo_url { get; set; }
        [JsonPropertyName("username")]
        public string username { get; set; }
    }
}