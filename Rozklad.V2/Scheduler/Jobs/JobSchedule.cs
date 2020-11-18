using System;
using System.Collections.Generic;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;

namespace Rozklad.V2.Scheduler.Jobs
{
    public class JobSchedule
    {
        
        public Type JobType { get; set; }
        public string CronExpression { get; set; }
        
        public FireTime FireTime { get; set; }

    }
}