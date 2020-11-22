using System;
using System.Linq;
using System.Threading.Tasks;
using Rozklad.V2.Helpers;
using Rozklad.V2.Pages;
using Rozklad.V2.Services;

namespace Rozklad.V2.Scheduler
{
    public class NotificationJob : INotificationJob
    {
        // Get all lessons at that time 
        // Call notificate method 
        private readonly IRozkladRepository _repository;
        private readonly ITelegramNotificationService _telegramNotifications;
        public NotificationJob(IRozkladRepository repository, ITelegramNotificationService telegramNotifications)
        {
            _repository = repository;
            _telegramNotifications = telegramNotifications;
        }

        public async Task Execute(FireTime fireTime)
        {
            // get all students with notifications 
            // get lessons for that time 
            // get notifications for push and telegram 
            var notifications = (await _repository.GetAllNotificationsByThisTime(fireTime)).ToList();
            var pushNotifications = notifications.Where(n => n.Type == "Push");
            var telegramNotifications = notifications.Where(n => n.Type == "Telegram");
            await _telegramNotifications.SendNotifications(telegramNotifications);

        }
    }
}