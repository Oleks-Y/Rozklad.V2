using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
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

        public JobsManager(IRecurringJobManager recurringJobManager, NotificationJob notificationJob,
            ISchedulerService schedulerService)
        {
            _recurringJobManager = recurringJobManager;
            _notificationJob = notificationJob;
            _schedulerService = schedulerService;
            _jobSchedules = new List<JobSchedule>();
            
            var defaultRetryFilter = GlobalJobFilters.Filters
                .FirstOrDefault(f => f.Instance is AutomaticRetryAttribute);

            if (defaultRetryFilter?.Instance != null)
            {
                // Todo test notifications with multiple users 
                // можливо, що розсилка буде надто повільна 
                // todo усю логіку варто жорстко переглянути на діри 
                // todo тествувати з великою кількістю користувачів 
                GlobalJobFilters.Filters.Remove(defaultRetryFilter.Instance);
            }
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute{ Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete});
        }

        public void InitJobs()
        {
            var jobSchedules = _schedulerService.GetJobSchedules().GetAwaiter().GetResult();
            foreach (var jobSchedule in jobSchedules.ToArray().AsParallel())
            {
                _recurringJobManager.AddOrUpdate(jobSchedule.Cron,
                    () => _notificationJob.Execute(jobSchedule.FireTime),
                    jobSchedule.Cron);
            }
            // todo: визначити, що відбувається, коли у студента обрані кілька предметів на даний час 
            this._jobSchedules = jobSchedules.ToArray();
        }

        public async Task RefreshJobs()
        {
            // delete all old schedules 
            // todo можливо, варто не видаляти усі роботи
            using (var connection = JobStorage.Current.GetConnection()) 
            {
                foreach (var recurringJob in StorageConnectionExtensions.GetRecurringJobs(connection)) 
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
            // Problem : every time ,when sending request for notificationController,
            // all schedules delete and new will be write, but we have only replace jobs  
            var jobSchedules = await _schedulerService.GetJobSchedules();
            foreach (var jobSchedule in jobSchedules)
            {
                _recurringJobManager.AddOrUpdate(jobSchedule.Cron,
                    () =>  _notificationJob.Execute(jobSchedule.FireTime),
                    jobSchedule.Cron);
            }

            _jobSchedules = jobSchedules;
        }
    }
}