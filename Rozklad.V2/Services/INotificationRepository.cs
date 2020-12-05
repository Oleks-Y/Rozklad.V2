using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Services
{
    public interface INotificationRepository
    {
        /// <summary>
        /// Method, that set notificationsFor student on 
        /// </summary>
        /// <param name="stuedntId"></param>
        /// <returns>List of firetimes, that need to add </returns>
        Task<IEnumerable<FireTime>> EnableNotifications(Guid stuedntId);
        /// <summary>
        /// Method, that set notifications for student off
        /// </summary>
        /// <param name="stuedntId"></param>
        /// <returns>List of firetimes, that need to remove</returns>
        Task<IEnumerable<FireTime>> DisableNotifications(Guid stuedntId);
    }
}