using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;
using Hangfire;
using Hangfire.Storage;
using Rozklad.V2.Services;

namespace Rozklad.V2.Scheduler
{
    public class __JobsManager
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly INotificationJob _notificationJob;

        private readonly ISchedulerService _schedulerService;
        // public only call method to update firetimes 

        private IEnumerable<JobSchedule> _jobSchedules;

        public __JobsManager(IRecurringJobManager recurringJobManager, NotificationJob notificationJob,
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
            this._jobSchedules = jobSchedules.ToArray();
        }
        // todo run job in parallel thread 
        // todo check if it slow database connection s
        public async Task RefreshJobs()
        {
            // delete all old schedules 
            // todo можливо, варто не видаляти усі роботи
            var connection = JobStorage.Current.GetConnection();
            var deletingTask = Task.Run(() => connection.GetRecurringJobs().ForAll(
                    reccuringJob =>
                    {
                        _recurringJobManager.RemoveIfExists(reccuringJob.Id);
                    }));
            // Problem : every time ,when sending request for notificationController,
            // all schedules delete and new will be write, but we have only replace jobs  
            // todo this a little slow 
            var jobSchedules = await _schedulerService.GetJobSchedules();
            // todo this is veeru slow 
            var addingTask = Task.Run(() => jobSchedules.ForAll((jobSchedule) => _recurringJobManager.AddOrUpdate(
                jobSchedule.Cron,
                () => _notificationJob.Execute(jobSchedule.FireTime),
                jobSchedule.Cron))
            );
            _jobSchedules = jobSchedules;
            await Task.WhenAll(deletingTask, addingTask);
        }
    }
}