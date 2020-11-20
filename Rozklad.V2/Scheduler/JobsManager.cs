using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Rozklad.V2.Services;

namespace Rozklad.V2.Scheduler
{
    public class JobsManager
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly INotificationJob _notificationJob;

        private readonly ISchedulerService _schedulerService;
        // public only call method to update firetimes 

        private IEnumerable<JobSchedule> _jobSchedules;

        public JobsManager(IRecurringJobManager recurringJobManager, INotificationJob notificationJob,
            ISchedulerService schedulerService)
        {
            _recurringJobManager = recurringJobManager;
            _notificationJob = notificationJob;
            _schedulerService = schedulerService;
        }
        
        public void InitJobs()
        {
            var jobSchedules = _schedulerService.GetJobSchedules();
            foreach (var jobSchedule in jobSchedules.ToArray().AsParallel())
            {
                _recurringJobManager.AddOrUpdate(jobSchedule.Cron,
                    () =>  _notificationJob.Execute(jobSchedule.FireTime),
                    jobSchedule.Cron);
                
            }

            _jobSchedules = jobSchedules;
        }

        public void RefreshJobs()
        {
            var jobSchedules = _schedulerService.GetJobSchedules();
            foreach (var jobSchedule in jobSchedules.ToArray().AsParallel())
            {
                _recurringJobManager.RemoveIfExists(jobSchedule.Cron);
                _recurringJobManager.AddOrUpdate(jobSchedule.Cron,
                    () =>  _notificationJob.Execute(jobSchedule.FireTime),
                    jobSchedule.Cron);
                
            }
            _jobSchedules = jobSchedules;
        }
    }
}