using System.Collections.Generic;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public interface ISchedulerService
    {
        IEnumerable<JobSchedule> GetJobSchedules();
    }
}