using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;
using Rozklad.V2.Services;

namespace Rozklad.V2.Controllers
{
    [ApiController]
    [Route("api/student/{studentId:guid}/timetable")]
    public class TimeTableController : ControllerBase
    {
        private IStudentService _studentService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IRozkladRepository _repository;

        public TimeTableController(IStudentService studentService, IMapper mapper, IOptions<AppSettings> appSettings,
            IRozkladRepository repository)
        {
            _studentService = studentService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetTimetableAsync(Guid studentId)
        {
            if (!_repository.StudentExists(studentId))
            {
                return NotFound();
            }

            // get lessons 
            // order lessons
            var student = await _repository.GetStudentAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }

            var lessons = await _repository.GetLessonsForStudent(studentId);

            return Ok(lessons);
        }
    }
}