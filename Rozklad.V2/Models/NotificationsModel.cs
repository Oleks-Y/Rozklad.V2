using System.ComponentModel.DataAnnotations;

namespace Rozklad.V2.Models
{
    public class NotificationsModel
    {
        public bool IsNotificationsOn { get; set; }
        
        [Range(5,30)]
        public int TimeBeforeLesson { get; set; }
    }
}