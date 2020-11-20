﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;
using Rozklad.V2.Scheduler;


namespace Rozklad.V2.Services
{
    public class SchedulerService : ISchedulerService
    {
    private readonly IRozkladRepository _repository;
    
    public SchedulerService(IRozkladRepository repository)
    {
        _repository = repository;
    }
    
    public  IEnumerable<JobSchedule> GetJobSchedules()
    {
        var fireTimes = _repository.GetAllNotificationsFireTimes().GetAwaiter().GetResult();
        var schedules = fireTimes.Select(f => 
            // new JobSchedule(typeof(NotificationJob), 
            //     calculateCronExpresion(f))
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
        // Todo error handling
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
        // Todo 
        // Ну це звичайно ще та х#йня,
        // треба додати можливість змінювати парність першого тижня з конфігурації
        // парні номера днів в місяці - перший тиждень 
        // непарні номера днів в місяці - другий тиждень 
        // todo in cron can add month day with "-"
        var cronString = $"0 {fireTime.Time.Minutes} {fireTime.Time.Hours} * * {dayName}";
    
        return cronString;
    }
    }
}