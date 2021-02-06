using System;
using Google.Apis.Auth;

namespace Rozklad.V2.Entities
{
    public class AuthentificateRequestGoogle
    {
        public GoogleJsonWebSignature.Payload User { get; set; }
        
        public Guid GroupId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}