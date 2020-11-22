using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface ITelegramNotificationService
    {
        Task SendNotifications(IEnumerable<Notification> notification);
    }
}