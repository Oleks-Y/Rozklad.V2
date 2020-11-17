using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Entities;
using Rozklad.V2.Models;
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
        public NotificationForStudentController(IRozkladRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> SetNotification(Guid studentId, [FromBody] NotificationsModel notificationsModel)
        {
            // check if student exists 
            // update notifications
            //return 204
            var studentExists = _repository.StudentExists(studentId);
            if (!studentExists)
            {
                return NotFound();
            }

            var notificationEntity = _mapper.Map<NotificationsSettings>(notificationsModel);
            notificationEntity.StudentId = studentId;
            await _repository.UpdateNotification(notificationEntity);
            await _repository.SaveAsync();
            return NoContent();

        }
    }
}