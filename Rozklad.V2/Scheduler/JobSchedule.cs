using Rozklad.V2.Helpers;

namespace Rozklad.V2.Scheduler
{
    public class JobSchedule
    {
        public string Cron { get; set; }

        public FireTime FireTime { get; set; }
        
    }
}