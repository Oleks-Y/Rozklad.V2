using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;
using Rozklad.V2.Services;

namespace Rozklad.V2.Controllers
{
    [Authorize]
    [Route("api/student/{studentId:guid}/subjects")]
    public class SubjectsForStudentController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IRozkladRepository _repository;

        public SubjectsForStudentController( IMapper mapper, 
            IRozkladRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // [HttpGet]
        // public async  Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjecs(Guid studentId)
        // {
        //     // Get student id 
        //     // Get student group
        //     // Get all subjects to hroup 
        //     // Check if there is banned subjects 
        //     if (string.IsNullOrEmpty(studentId.ToString()))
        //     {
        //         return NotFound();
        //     }
        //     var student = await _repository.GetStudent(studentId);
        //     if (student == null)
        //     {
        //         return BadRequest();
        //     }
        //     if (student?.Group == null)
        //     {
        //         return BadRequest();
        //     }
        //     var group = student.Group;
        //     var subjectsDtos = group.Subjects.Select(s => _mapper.Map<SubjectDto>(s)).ToList();
        //     
        //     return subjectsDtos;
        // }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjecs(Guid studentId)
        {
            if (string.IsNullOrEmpty(studentId.ToString()))
            {
                return NotFound();
            }
            var student = await _repository.GetStudentAsync(studentId);
            if (student == null)
            {
                return BadRequest();
            }
            if (student?.Group == null)
            {
                return BadRequest();
            }

            var subjects = await _repository.GetSubjectsForStudentAsync(student.Id);
            var subjectsDtos = subjects.Select(s => _mapper.Map<SubjectDto>(s)).ToList();

            return subjectsDtos;

        }
        [HttpGet("disabled")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetDisabledSubjecs(Guid studentId)
        {
            if (string.IsNullOrEmpty(studentId.ToString()))
            {
                return NotFound();
            }
            var student = await _repository.GetStudentAsync(studentId);
            if (student == null)
            {
                return BadRequest();
            }
            if (student?.Group == null)
            {
                return BadRequest();
            }

            var subjects = await _repository.GetDisabledSubjectsAsync(studentId);
            var subjectsDtos = subjects.Select(s => _mapper.Map<SubjectDto>(s)).ToList();

            return subjectsDtos;
        }
        [HttpPatch("{subjectId:guid}/disable")]
        public async Task<IActionResult> DisableSubjects(Guid studentId, Guid subjectId)
        {
            if (string.IsNullOrEmpty(studentId.ToString()))
            {
                return NotFound();
            }
            var student = await _repository.GetStudentAsync(studentId);
            if (student == null)
            {
                return BadRequest("Student don`t exists");
            }

            var subject = await _repository.GetSubjectAsync(subjectId);
            if (subject == null)
            {
                return BadRequest("Subject won`t exists");
            }
            // disable 
            await _repository.DisableSubjectAsync(student.Id, subject.Id);
            await _repository.SaveAsync();

            return NoContent();
        } 
        [HttpPatch("{subjectId:guid}/enable")]
        public async Task<IActionResult> EnableSubjects(Guid studentId, Guid subjectId)
        {
            if (string.IsNullOrEmpty(studentId.ToString()))
            {
                return NotFound();
            }
            var student = await _repository.GetStudentAsync(studentId);
            if (student == null)
            {
                return BadRequest("Student don`t exists");
            }

            var subject = await _repository.GetSubjectAsync(subjectId);
            if (subject == null)
            {
                return BadRequest("Subject won`t exists");
            }
            // disable 
            _repository.EnableSubject(student.Id, subject.Id);
            await _repository.SaveAsync();

            return NoContent();
        }
        
    }
}