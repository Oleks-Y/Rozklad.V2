using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface ITelegramNotificationService
    {
        void SendNotifications(IEnumerable<Notification> notification);
    }
}