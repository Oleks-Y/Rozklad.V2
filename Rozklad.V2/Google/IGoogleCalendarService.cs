using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Google
{
    public interface IGoogleCalendarService
    {
        Task SetNotificationsInCalendar(CalendarSettings settings);
    }
}