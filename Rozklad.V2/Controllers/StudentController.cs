using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rozklad.V2.Entities;
using Rozklad.V2.Exceptions;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;
using Rozklad.V2.Services;

namespace Rozklad.V2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private IStudentService _studentService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IRozkladRepository _repository;

        public StudentController(IStudentService studentService, IMapper mapper, IOptions<AppSettings> appSettings,
            IRozkladRepository repository)
        {
            _studentService = studentService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // Todo create repository 
            // Todo check for group 
            var student = _mapper.Map<Student>(model);
            var group = _repository.GetGroupByName(model.Group);
            if ( group== null)
            {
                return BadRequest(new {message = "Group not exist!"});
            }

            student.GroupId = group.Id;
            try
            {
                _studentService.Create(student, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] Models.AuthentificateModel model)
        {
            var user = _studentService.Authentificate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new {message = "Username or password is incorrect"});
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var authDto = _mapper.Map<AuthentificateDto>(user);
            authDto.Token = tokenString;
            return Ok(authDto);
        }
        // [Authorize]
        // [HttpPatch("{studentId:guid}")]
        // public async  Task<ActionResult> PatchStudent(Guid studentId,
        //     [FromBody] JsonPatchDocument<StudentForUpdateDto> patchDocument)
        // {
        //     if (patchDocument == null)
        //     {
        //         return BadRequest();
        //     }
        //     var studentFromRepo = await _repository.GetStudentAsync(studentId);
        //     if (studentFromRepo == null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var studentToPatch = _mapper.Map<StudentForUpdateDto>(studentFromRepo); 
        //     patchDocument.ApplyTo(studentToPatch, ModelState);
        //     if (!TryValidateModel(studentToPatch))
        //     {
        //         return ValidationProblem();
        //     }
        //
        //     _mapper.Map(studentToPatch, studentFromRepo);
        //     
        //     _repository.UpdateStudent(studentFromRepo);
        //
        //     await _repository.SaveAsync();
        //
        //     return NoContent();
        //
        //
        // }
    }
}