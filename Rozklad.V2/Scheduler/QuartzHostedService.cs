using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Rozklad.V2.Helpers;
using Rozklad.V2.Scheduler.Jobs;
using Rozklad.V2.Services;

namespace Rozklad.V2.Scheduler
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly ISchedulerService _schedulerService;
        private IEnumerable<JobSchedule> _jobSchedules;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            SingletonJobFactory jobFactory,
            IServiceProvider provider
        )
        {
            // Todo add tests to it 
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            var scope = provider.CreateScope();
            _schedulerService = scope.ServiceProvider.GetService<ISchedulerService>();
            RefreshSchedules();
        }
        // Todo use event on disabled subjects or notifications changed
        public void RefreshSchedules()
        {
            _jobSchedules = _schedulerService.GetJobSchedules();
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .UsingJobData("FireTime", JsonConvert.SerializeObject(schedule.FireTime))
                .Build();
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.CronExpression}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }
}