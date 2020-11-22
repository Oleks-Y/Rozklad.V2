using System.Threading.Tasks;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Scheduler
{
    public interface INotificationJob
    {
        public Task Execute(FireTime fireTime);
    }
}