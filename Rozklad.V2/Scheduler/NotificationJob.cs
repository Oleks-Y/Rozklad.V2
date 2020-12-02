using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NotificationJob> _logger;
        public NotificationJob(IRozkladRepository repository, ITelegramNotificationService telegramNotifications, ILogger<NotificationJob> logger)
        {
            _repository = repository;
            _telegramNotifications = telegramNotifications;
            _logger = logger;
        }
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete,Order = 0)]
        public  void Execute(FireTime fireTime)
        {
            // Todo add week number 
            // don`t run job, if it too late 
            //todo  enable it in prod 
            var now = DateTime.Now;
            // check by day 
            if ((long)now.DayOfWeek != fireTime.NumberOfDay)
            {
                _logger.LogWarning($"Job {fireTime.Time} {fireTime.NumberOfDay} {fireTime.NumberOfWeek } runs late, stop job");
                return;
            }
            // check by time 
            if (now.TimeOfDay>fireTime.Time.Add(TimeSpan.FromMinutes(10)) || now.TimeOfDay<fireTime.Time.Subtract(TimeSpan.FromMinutes(10)))
            {
                _logger.LogWarning($"Job {fireTime.Time} {fireTime.NumberOfDay} {fireTime.NumberOfWeek } runs late, stop job");
                return;                
            }
            // todo check by week 
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