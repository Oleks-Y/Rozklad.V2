using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3.Data;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Google
{
    public interface IGoogleCalendarService
    {
        Task<IEnumerable<Event>> SetNotificationsInCalendar(CalendarSettings settings);

        Task UpdateNottificationsInCalednar(CalendarSettings settings, IEnumerable<string> eventsToUpdate);
        Task DeleteNotificationsInCalendar(CalendarSettings setting, IEnumerable<string> eventsToDelete);
        Task<string> GetCalendarIdForUser(string accessToken);
    }
}