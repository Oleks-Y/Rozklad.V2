using Rozklad.V2.Entities;

namespace Rozklad.V2.Services
{
    public interface ITelegramNotificationService
    {
        void SendNotification(Notification notification);
    }
}