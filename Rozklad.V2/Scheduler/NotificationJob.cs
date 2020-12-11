using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;
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
            // don`t run job, if it too late 
            //todo  enable it in prod 
            var now = DateTime.Now;
            // check by day 
            // if ((long)now.DayOfWeek != fireTime.NumberOfDay)
            // {
            //     _logger.LogWarning($"Job {fireTime.Time} {fireTime.NumberOfDay} {fireTime.NumberOfWeek } runs late, stop job");
            //     return;
            // }
            // // check by time 
            // if (now.TimeOfDay>fireTime.Time.Add(TimeSpan.FromMinutes(10)) || now.TimeOfDay<fireTime.Time.Subtract(TimeSpan.FromMinutes(10)))
            // {
            //     _logger.LogWarning($"Job {fireTime.Time} {fireTime.NumberOfDay} {fireTime.NumberOfWeek } runs late, stop job");
            //     return;                
            // }
            var notifications = _repository.GetAllNotificationsByThisTime(fireTime)
                .GroupBy(n=>n.StudentId)
                .SelectMany(g=>g.DistinctBy(n=>n.Lesson.Id))
                .ToList();
            var pushNotifications = notifications.Where(n => n.Type == "Push").ToList();
            var telegramNotifications = notifications.Where(n => n.Type == "Telegram").ToList();
            _telegramNotifications.SendNotifications(telegramNotifications);

        }
    }
}