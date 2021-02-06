using System;
using Google.Apis.Auth;

namespace Rozklad.V2.Models
{
    public class GoogleAuthModel
    {
        public string tokenId { get; set; }
        
        public Guid GroupId { get; set; }

        public string AccessToken { get; set; }
    }
}