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
        private readonly IRozkladRepository _repository;
        private IMapper _mapper;
        private readonly IJobManager _jobsManager;
        public NotificationForStudentController(IRozkladRepository repository, IMapper mapper, IJobManager jobsManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> SetNotifications(Guid studentId,
            [FromBody] NotificationsModel notificationsModel)
        {
            var student = await _repository.GetStudentWithNotification(studentId);
            if (student == null)
            {
                return NotFound();
            }
            
            var notificationentity = _mapper.Map<NotificationsSettings>(notificationsModel);
            notificationentity.StudentId= studentId;
            await _repository.UpdateNotification(notificationentity);
            await _repository.SaveAsync();
            var telegramDataExists = _repository.UserTelegramDataExists(studentId);
            if (notificationsModel.NotificationType == "Telegram" && !telegramDataExists)
            {
                return Ok(new {message = "Для роботи нотифікацій через телеграм бот, потрібно авторизуватись через телеграм, зайти в телеграмм бот і натиснути /start"});
            }
            return NoContent();
        }
    }
}