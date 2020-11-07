using System;
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
    [ApiController]
    [Route("api/subject")]
    public class SubjectsController : ControllerBase
    {
        private IStudentService _studentService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IRozkladRepository _repository;

        public SubjectsController(IStudentService studentService, IMapper mapper, IOptions<AppSettings> appSettings,
            IRozkladRepository repository)
        {
            _studentService = studentService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _repository = repository;
        }
        [HttpPatch("{subjectId:guid}")]
        public async Task<ActionResult> PatchSubject(Guid subjectId, [FromBody] JsonPatchDocument<SubjectToUpdate> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var subjectFromRepo = await _repository.GetSubjectAsync(subjectId);
            if (subjectFromRepo == null)
            {
                return NotFound();
            }

            var subjectToPatch = _mapper.Map<SubjectToUpdate>(subjectFromRepo);
            patchDocument.ApplyTo(subjectToPatch, ModelState);
            if (!TryValidateModel(subjectToPatch))
            {
                return ValidationProblem();
            }

            _mapper.Map(subjectToPatch, subjectFromRepo);
            
            _repository.UpdateSubject(subjectFromRepo);
            
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}