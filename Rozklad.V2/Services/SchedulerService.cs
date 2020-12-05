using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly IRozkladRepository _repository;
        private readonly IJobManager _jobManager;
        public SchedulerService(IRozkladRepository repository, IJobManager jobManager)
        {
            _repository = repository;
            _jobManager = jobManager;
        }
        
        public async Task AddNotificationsForStudent(Guid studentId)
        {
            var schedules = await getSchedulesForStudent(studentId);
            await _jobManager.AddJobs(schedules);
        }

        public async Task RemoveNotificationsForStudent(Guid studentId)
        {
            var schedules = await getSchedulesForStudent(studentId);
            await _jobManager.RemoveJobs(schedules);
        }
        
        private async Task<IEnumerable<JobSchedule>> getSchedulesForStudent(Guid studentId)
        {
            var fireTimes = await _repository.GetFireTimesForStudent(studentId);
            var schedules = fireTimes.Select(f =>
                new JobSchedule
                {
                    Cron = calculateCronExpresion(f),
                    FireTime = f
                }
            );
            // delete, if cronn count is more > 1 
            var jobSchedules = schedules as JobSchedule[] ?? schedules.ToArray();
            var distinctSchedules = jobSchedules.DistinctBy(s=>s.Cron).ToArray();

            return distinctSchedules;
        }
        private static string calculateCronExpresion(FireTime fireTime)
        {
            // generate expressions 
            // {seconds} {minutes} {hours} ? {DAY}#{numbersOfDayAtMonth}
            var dayName = fireTime.NumberOfDay switch
            {
                1 => "MON",
                2 => "TUE",
                3 => "WED",
                4 => "THU",
                5 => "FRI",
                6 => "SAT",
                _ => ""
            };
            var cronString = $"{fireTime.Time.Minutes} {fireTime.Time.Hours} * * {dayName}";
    
            return cronString;
        }

        
    }
}