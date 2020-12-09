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
        // todo rewrite this 
        private readonly IJobManager _jobsManager;
        public NotificationForStudentController(IRozkladRepository repository, IMapper mapper, IJobManager jobsManager)
        {
            _repository = repository;
            _mapper = mapper;
            // _jobsManager = jobsManager;
        }

        // [HttpPost]
        // public async Task<ActionResult> SetNotification(Guid studentId, [FromBody] NotificationsModel notificationsModel)
        // {
        //     if (notificationsModel == null) throw new ArgumentNullException(nameof(notificationsModel));
        //     // check if student exists 
        //     // update notifications
        //     //return 204
        //     var studentExists = _repository.StudentExists(studentId);
        //     if (!studentExists)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var notificationEntity = _mapper.Map<NotificationsSettings>(notificationsModel);
        //     notificationEntity.StudentId = studentId;
        //     await _repository.UpdateNotification(notificationEntity);
        //     await _repository.SaveAsync();
        //     var telegramDataExists = _repository.UserTelegramDataExists(studentId);
        //     if (notificationsModel.NotificationType == "Telegram" && !telegramDataExists)
        //     {
        //         return Ok(new {message = "Для роботи нотифікацій через телеграм бот, потрібно авторизуватись через телеграм, зайти в телеграмм бот і натиснути /start"});
        //     }
        //     return NoContent();
        //     // if student telegram chat not exists, return it to client
        // }
        [HttpPost]
        public async Task<ActionResult> SetNotifications(Guid studentId,
            [FromBody] NotificationsModel notificationsModel)
        {
            var student = _repository.GetStudentWithNotification(studentId);
            if (student == null)
            {
                return NotFound();
            }
            
            var notificationentity = _mapper.Map<NotificationsSettings>(notificationsModel);
            notificationentity.StudentId= studentId;
            await _repository.UpdateNotification(notificationentity);
            await _repository.SaveAsync();
            // todo test notifications 
            var telegramDataExists = _repository.UserTelegramDataExists(studentId);
            if (notificationsModel.NotificationType == "Telegram" && !telegramDataExists)
            {
                return Ok(new {message = "Для роботи нотифікацій через телеграм бот, потрібно авторизуватись через телеграм, зайти в телеграмм бот і натиснути /start"});
            }
            return NoContent();
        }
    }
}