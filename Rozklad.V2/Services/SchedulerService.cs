using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IJobManager _jobManager;
        public SchedulerService(IJobManager jobManager, INotificationRepository notificationRepository)
        {
            _jobManager = jobManager;
            _notificationRepository = notificationRepository;
        }
        
        
        private IEnumerable<JobSchedule> getSchedules(IEnumerable<FireTime> fireTimes)
        {
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
        /// <summary>
        /// Command, that set ALL jobs, when will be sended notificatinos 
        /// </summary>
        /// <returns></returns>
        
       public void InitialalizeJobs()
        {
            var fireTimes = new List<FireTime>();
            foreach (int week in NotificationsConfig.Weeks)
            foreach (var day in NotificationsConfig.Days)
            {
                foreach (var lessonTime in NotificationsConfig.LessonsTimes)
                {
                    foreach (var timesBefforeLesson in NotificationsConfig.TimesBeforeLesson)
                    {
                        fireTimes.Add(new FireTime
                        {
                            NumberOfWeek = week,
                            NumberOfDay = day,
                            LessonTime = lessonTime,
                            Time = lessonTime.Add(new TimeSpan(0,-timesBefforeLesson,0))
                            
                        });
                    }
                }
            }

            var timeSchedules = fireTimes.Select(f=>new JobSchedule
            {
                Cron = calculateCronExpresion(f),
                FireTime = f
            });

            _jobManager.AddJobs(timeSchedules);
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
            string cronString;
            if (NotificationsConfig.IsFirstWeekEven)
            {
                cronString = fireTime.NumberOfWeek == 1 ? 
                    $"{fireTime.Time.Minutes} {fireTime.Time.Hours} 1-7,15-21,29-31 * {dayName}" : 
                    $"{fireTime.Time.Minutes} {fireTime.Time.Hours} 8-14,22-28 * {dayName}";            }
            else                                                                 {
                cronString = fireTime.NumberOfWeek == 2 ?
                    $"{fireTime.Time.Minutes} {fireTime.Time.Hours} 1-7,15-21,29-31 * {dayName}" : 
                    $"{fireTime.Time.Minutes} {fireTime.Time.Hours} 8-14,22-28 * {dayName}";            }
            return cronString;
        }                                                            
                                                                             
    }
}