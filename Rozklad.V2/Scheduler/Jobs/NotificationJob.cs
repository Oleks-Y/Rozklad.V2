using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using Rozklad.V2.Helpers;
using Rozklad.V2.Services;

namespace Rozklad.V2.Scheduler.Jobs
{
    public class NotificationJob : IJob
    {
        private readonly ILogger<NotificationJob> _logger;
        private readonly IRozkladRepository _repository;
        public NotificationJob(ILogger<NotificationJob> logger,IServiceProvider provider)
        {
            _logger = logger;
            var scope = provider.CreateScope();
            _repository = scope.ServiceProvider.GetService<IRozkladRepository>();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            // get fire time 
            
            // call notificationService 
            var fireTimeString = context.MergedJobDataMap.GetString("FireTime");
            _logger.LogInformation("Here will be sent notifications to all ");
            var fireTime = JsonConvert.DeserializeObject<FireTime>(fireTimeString, new JsonSerializerSettings
            {
                Error = (object sender, ErrorEventArgs args) =>
                {
                    _logger.LogError(@"Wrong object was in FireTime, exception while deserialize");
                }
            });
            // get all lessons with notifications on this time 

            var notifications = await _repository.GetAllNotificationsByThisTime(fireTime.NumberOfWeek,
                fireTime.NumberOfDay, fireTime.LessonTime);
            foreach (var notification in notifications)
            {
                if (notification.Type=="Telegram")
                {
                    // send message in telegram bot 
                }
                else
                {
                    // send push notification
                }
            }

        }
    }
}