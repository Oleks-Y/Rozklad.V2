﻿using System;
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
        private readonly JobsManager _jobsManager;
        public NotificationForStudentController(IRozkladRepository repository, IMapper mapper, JobsManager jobsManager)
        {
            _repository = repository;
            _mapper = mapper;
            _jobsManager = jobsManager;
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
            _jobsManager.RefreshJobs();
            await _repository.SaveAsync();
            return NoContent();

        }
    }
}