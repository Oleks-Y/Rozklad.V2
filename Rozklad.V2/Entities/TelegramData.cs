using System;

namespace Rozklad.V2.Entities
{
    /// <summary>
    /// Data, that needed for contact user in telegram 
    /// </summary>
    public class TelegramData
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        
        public long? TelegramId { get; set; } 
        
        public long? TelegramChatId { get; set; }
    }
}