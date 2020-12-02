using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types.Enums;

namespace Rozklad.V2.Models
{
    public class NotificationsModel
    {
        [Required]
        public bool IsNotificationsOn { get; set; }
        
        [Required]
        [Range(5,30, ErrorMessage = "TimeBeforeLesson must be 5,10,15,20,25 or 30")]
        public int TimeBeforeLesson { get; set; } 
        
        [Required] public string NotificationType { get; set; }
    }
}