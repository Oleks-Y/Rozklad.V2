using System;
using Rozklad.V2.Models;

namespace Rozklad.V2.Entities
{
    public class AuthenticateRequestTelegram
    {
        public TelegramUser TelegramUser { get; set; }

        public Guid GroupId { get; set; }
    }
}