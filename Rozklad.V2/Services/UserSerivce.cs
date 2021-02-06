using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Google;
using Rozklad.V2.Helpers;
using Telegram.Bot.Types;

namespace Rozklad.V2.Services
{
    public class UserSerivce : IUserSerice
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IGoogleCalendarService _calendarService;
        public UserSerivce(ApplicationDbContext context, AppSettings appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }
        // todo користувачі можуть змінювати імена і юзернейми в телеграмі 
        // todo додати можливість змінити групу 
        // public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        // {
        //     //get student from db by username 
        //     // generate jwt token 
        //     // generate refresh token 
        //     // save refresh token 
        // }

        public AuthenticateResponse AuthenticateWithTelegram(AuthenticateRequestTelegram model, string ipAddress)
        {
            // Check if student exists by id
            var student = _context.Students
                .Include("Group")
                .Include("RefreshTokens")
                .SingleOrDefault(s => s.Telegram_Id == model.TelegramUser.id);
            if (student == null)
            {
                // if not exists register and auth
                student = new Student
                {
                    Id = Guid.NewGuid(),
                    Username = model.TelegramUser.username,
                    FirstName = model.TelegramUser.first_name,
                    LastName = model.TelegramUser.last_name,
                    GroupId = model.GroupId,
                    Telegram_Id = model.TelegramUser.id
                };
                _context.Students.Add(student);
                _context.SaveChanges();
            }

            // if exists-  auth 
            if (student.GroupId != model.GroupId)
            {
                return null;
            }

            var jwt = generateJwtToken(student);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token 
            student.RefreshTokens.Add(refreshToken);
            _context.Update(student);
            _context.SaveChanges();


            return new AuthenticateResponse {Student = student, JwtToken = jwt, RefreshToken = refreshToken.Token};
        }

        public async Task<AuthenticateResponse> AuthentificateWithGoogle(AuthentificateRequestGoogle model, string ipAddress)
        {
            //Same as telegram logic
            var student = _context.Students
                .Include("Group")
                .Include("RefreshTokens")
                // HERE USERNAME IS EMAIL 
                .SingleOrDefault(s => s.Username == model.User.Email);
            if (student == null)
            {
                // if user not exists in database - create new one 
                
                student = new Student
                {
                    Id = Guid.NewGuid(),
                    Username = model.User.Email,
                    FirstName = model.User.GivenName,
                    LastName = model.User.FamilyName,
                    GroupId = model.GroupId,
                };
                // add googleData of user to database 
                await addGoogleData(student, model);
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
            }
            
            // if exists-  auth 
            if (student.GroupId != model.GroupId)
            {
                return null;
            }

            var jwt = generateJwtToken(student);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token 
            // TODO newly added user don`t have refresh tokens 
            // student.RefreshTokens.Add(refreshToken);
            _context.Update(student);
            await _context.SaveChangesAsync();
    
            return new AuthenticateResponse
            {
                Student = student, 
                JwtToken = jwt, 
                RefreshToken = refreshToken.Token
            };
        }
        public Student GetById(Guid studentId)
        {
            return _context.Students.Find(studentId);
        }

        //todo registration method 
        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var student = _context.Students.Include("RefreshTokens").SingleOrDefault(s => s.RefreshTokens.Any(t => t.Token == token));

            if (student == null) return null;

            var refreshToken = student.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active 
            if (!refreshToken.IsActive) return null;

            // replace old refresh toekn with a new one and save 
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            student.RefreshTokens.Add(newRefreshToken);
            _context.Update(student);
            _context.SaveChanges();

            // generate new jwt 
            var jwtToken = generateJwtToken(student);

            return new AuthenticateResponse
            {
                Student = student,
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var student = _context.Students.Include("RefreshTokens").SingleOrDefault(s=>s.RefreshTokens.Any(t=>t.Token == token));
            
            // return false if no user found with token 
            if (student == null) return false;
            
            var refreshToken = student.RefreshTokens.Single(x => x.Token == token);
            
            // return false if token is not active
            if (!refreshToken.IsActive) return false;
            
            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(student);
            _context.SaveChanges();

            return true;
        }

        private async Task addGoogleData(Student student,AuthentificateRequestGoogle model)
        {
            var googleData = new GoogleData()
            {
                Id = Guid.NewGuid(),
                Email = model.User.Email,
                StudentId = student.Id,
                CalendarId = await _calendarService.GetCalendarIdForUser(model.AccessToken),
                RefreshToken = model.RefreshToken
            };
            await _context.GoogleData.AddAsync(googleData);
            await _context.SaveChangesAsync();
        }
        // helper methods

        private string generateJwtToken(Student user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}