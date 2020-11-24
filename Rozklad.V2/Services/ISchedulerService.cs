using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public interface ISchedulerService
    {
        Task<IEnumerable<JobSchedule>> GetJobSchedules();
    }
}