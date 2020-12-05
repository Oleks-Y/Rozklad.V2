using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public interface ISchedulerService
    {
        Task AddNotificationsForStudent(Guid studentId);

        Task RemoveNotificationsForStudent(Guid studentId);
    }
}