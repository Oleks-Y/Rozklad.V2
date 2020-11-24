using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
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
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete,Order = 0)]
        public  void Execute(FireTime fireTime)
        {
            var notifications = _repository.GetAllNotificationsByThisTime(fireTime).ToList();
            var pushNotifications = notifications.Where(n => n.Type == "Push").ToList();
            var telegramNotifications = notifications.Where(n => n.Type == "Telegram").ToList();
            // This method cause null exception
            // I can`t properly debug it 
            // But anyway, it works,
            // So if AutomaticRetry is selected, everything goes fine
            _telegramNotifications.SendNotifications(telegramNotifications);

        }
    }
}