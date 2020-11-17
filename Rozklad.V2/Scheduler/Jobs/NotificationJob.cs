using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Rozklad.V2.Scheduler.Jobs
{
    public class NotificationJob : IJob
    {
        private readonly ILogger<NotificationJob> _logger;
        public NotificationJob(ILogger<NotificationJob> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Here will be sent notifications to all ");
            return Task.CompletedTask;
        }
    }
}