using System;

namespace Rozklad.V2.Entities
{
    public class TelegramData
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        
        public long? TelegramId { get; set; } 
        
        public long? TelegramChatId { get; set; }
    }
}