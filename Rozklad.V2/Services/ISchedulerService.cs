using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Scheduler.Jobs;

namespace Rozklad.V2.Services
{
    public interface ISchedulerService
    {
        IEnumerable<JobSchedule> GetJobSchedules();
    }
}