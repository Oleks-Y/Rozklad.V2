﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Entities;
using Rozklad.V2.Models;
using Rozklad.V2.Services;
using Serilog;

namespace Rozklad.V2.Controllers
{
    
    [Authorize]
    [ApiController]
    [Route("api/student")]
    public class AuthController : ControllerBase
    {
        // todo allows student change telegram account  
        // need to secure way to change telegram account
        private readonly IUserSerice _userSerice;
        private readonly IMapper _mapper;
        private readonly IRozkladRepository _repository;
        public AuthController(IUserSerice userSerice, IMapper mapper, IRozkladRepository repository)
        {
            _userSerice = userSerice;
            _mapper = mapper;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet("group")]
        public async Task<IActionResult> GetStudentGroup([FromBody] TelegramUser model)
        {
            // get student by telegram id 
            // get group 
            // if group not exists - 404
            var student = await _repository.GetUserByTelegramId((long)model.id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(new GroupDto
            {
                GroupName = student.Group.Group_Name
            });
        }
        
        // todo changing group | when group chnaged, need delete all muted and disabled subjects for student 
        // todo changing telegram account 
        // todo some users in telegram don`t have username 
        [AllowAnonymous]
        [HttpPost("telegram")]
        public IActionResult AuthWithTelegram([FromBody] TelegramAuthModel model)
        {
            // todo Validate telegram request 
            var group = _repository.GetGroupByName(model.Group);
            if ( group== null)
            {
                return BadRequest(new {message = "Group not exist!"});
            }
            var authReuqest = new AuthenticateRequestTelegram
            {
                GroupId = group.Id,
                TelegramUser = model.TelegramUser
            };
            var authResult = _userSerice.AuthenticateWithTelegram(authReuqest,ipAddress());
            if (authResult == null)
            {
                return BadRequest("Group is not match user group!");
            }
            
            setTokenCookie(authResult.RefreshToken);

            return Ok(new AuthentificateDto
            {
                Id = authResult.Student.Id,
                Group = authResult.Student.Group.Group_Name,
                Username = authResult.Student.Username,
                FirstName = authResult.Student.FirstName,
                LastName = authResult.Student.LastName,
                Token = authResult.JwtToken,
                RefreshToken = authResult.RefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthModel model)
        {
            Log.Information("userView = "+ model.tokenId);
            GoogleJsonWebSignature.Payload payload =
               await GoogleJsonWebSignature.ValidateAsync(model.tokenId, new GoogleJsonWebSignature.ValidationSettings());

            var authRequest = new AuthentificateRequestGoogle()
            {
                User = payload,
                GroupId = model.GroupId
            };
            var authResult = await _userSerice.AuthentificateWithGoogle(authRequest, ipAddress());
            if (authResult == null)
            {
                return BadRequest("Group is not match user group!");
            }
            
            setTokenCookie(authResult.RefreshToken);

            return Ok(new AuthentificateDto
            {
                Id = authResult.Student.Id,
                Group = authResult.Student.Group.Group_Name,
                Username = authResult.Student.Username,
                FirstName = authResult.Student.FirstName,
                LastName = authResult.Student.LastName,
                Token = authResult.JwtToken,
                RefreshToken = authResult.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userSerice.RefreshToken(refreshToken, ipAddress());

            if (response == null)
                return Unauthorized(new {message = "Invalid token"});
            
            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new {message = "Token is required"});

            var response = _userSerice.RevokeToken(token, ipAddress());

            if (!response)
                return NotFound(new {message = "Token not found"});

            return Ok(new {nessage = "Token revoked"});
        }
        
        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}