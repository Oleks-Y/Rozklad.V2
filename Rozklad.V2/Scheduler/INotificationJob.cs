using System.Threading.Tasks;
using Hangfire;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Scheduler
{
    public interface INotificationJob
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void Execute(FireTime fireTime);
    }
}