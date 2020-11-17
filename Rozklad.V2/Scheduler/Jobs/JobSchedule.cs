using System;
using System.Collections.Generic;
using Rozklad.V2.Entities;
using Rozklad.V2.Models;

namespace Rozklad.V2.Scheduler.Jobs
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
        
        
    }
}