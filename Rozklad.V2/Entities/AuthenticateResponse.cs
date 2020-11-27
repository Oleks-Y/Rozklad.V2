using Newtonsoft.Json;

namespace Rozklad.V2.Entities
{
    public class AuthenticateResponse
    {
        public Student Student { get; set; }
        
        public string JwtToken { get; set; }
        
        [JsonIgnore] public string RefreshToken { get; set; }
        
    }
}