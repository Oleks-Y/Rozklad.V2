﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper.Internal;
using Hangfire;
using Rozklad.V2.Services;

namespace Rozklad.V2.Scheduler
{
    public class JobManager : IJobManager
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly INotificationJob _notificationJob;

        public JobManager(IRecurringJobManager recurringJobManager, INotificationJob notificationJob)
        {
            _recurringJobManager = recurringJobManager;
            _notificationJob = notificationJob;
        }

        public async Task AddJobs(IEnumerable<JobSchedule> schedules)
        {
            await Task.Run(() => schedules.ForAll(schedule => _recurringJobManager.AddOrUpdate(
                recurringJobId: schedule.Cron,
                methodCall:     ()=>_notificationJob.Execute(schedule.FireTime),
                cronExpression: schedule.Cron)));
        }

        public async Task RemoveJobs(IEnumerable<JobSchedule> schedules)
        {
            await Task.Run(() => schedules.ForAll(schedule => _recurringJobManager.RemoveIfExists(
                recurringJobId: schedule.Cron)));
        }
    }
}