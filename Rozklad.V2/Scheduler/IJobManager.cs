using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rozklad.V2.Scheduler
{
    public interface IJobManager
    {
        Task AddJobs(IEnumerable<JobSchedule> schedules);

        Task RemoveJobs(IEnumerable<JobSchedule> schedules);
    }
}