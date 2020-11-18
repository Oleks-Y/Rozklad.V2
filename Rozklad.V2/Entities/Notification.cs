using System;

namespace Rozklad.V2.Entities
{
    public class Notification
    {

        public Guid StudentId { get; set; }

        public Lesson Lesson { get; set; }
        // "Push" or "Telegram"
        public string Type { get; set; }
    }
}