using System.Collections.Generic;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Helpers
{
    public class CalendarSettings
    {
        public NotificationsSettings NotificationsSettings { get; set; }

        public string CalendarId { get; set; }

        public IEnumerable<Lesson> Lessons;
        
        public string AccessToken { get; set; }
    }
}