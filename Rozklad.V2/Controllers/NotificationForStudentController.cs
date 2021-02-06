using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Entities;
using Rozklad.V2.Models;
using Rozklad.V2.Scheduler;
using Rozklad.V2.Services;

namespace Rozklad.V2.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/student/{studentId:guid}/notifications")]
    public class NotificationForStudentController : ControllerBase
    {
        // Controller, where notifications can be on / off 
        private readonly IRozkladRepository _rozkladRepository;
        private INotificationRepository _notificationRepository;
        private IMapper _mapper;
        private readonly IJobManager _jobsManager;
        public NotificationForStudentController(INotificationRepository notificationRepository, IRozkladRepository _rozkladRepository, IMapper mapper)
        {
            this._rozkladRepository = _rozkladRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> SetNotifications(Guid studentId,
            [FromBody] NotificationsModel notificationsModel)
        {
            var student = await _rozkladRepository.GetStudentWithNotification(studentId);
            if (student == null)
            {
                return NotFound();
            }
            //
            var notificationentity = _mapper.Map<NotificationsSettings>(notificationsModel);
            notificationentity.StudentId= studentId;
            await _notificationRepository.SetNotificationSettings(notificationentity);
            return NoContent();
        }
    }
}