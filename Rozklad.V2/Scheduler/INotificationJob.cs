using Rozklad.V2.Helpers;

namespace Rozklad.V2.Scheduler
{
    public interface INotificationJob
    {
        public void Execute(FireTime fireTime);
    }
}