using System;
using System.ComponentModel.DataAnnotations;

namespace Rozklad.V2.Entities
{
    public class NotificationsSettings
    {
        [Key] public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public bool IsNotificationsOn { get; set; }
        
        public int TimeBeforeLesson { get; set; }
        // "Push" || "Telegram"
        public string NotificationType { get; set; }
    }
}