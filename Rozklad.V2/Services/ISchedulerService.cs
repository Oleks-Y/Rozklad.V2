using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Entities;
using Rozklad.V2.Scheduler;

namespace Rozklad.V2.Services
{
    public interface ISchedulerService
    {
        void InitialalizeJobs();
    }
}