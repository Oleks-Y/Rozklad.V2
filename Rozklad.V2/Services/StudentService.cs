using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Rozklad.V2.DataAccess;
using Rozklad.V2.Entities;
using Rozklad.V2.Exceptions;

namespace Rozklad.V2.Services
{
    public class StudentService : IStudentService
    {
        public ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Student Authentificate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var student = _context.Students.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (student == null)
                return null;

            // check if password is correct
            return !VerifyPasswordHash(password, student.PasswordHash, student.PasswordSalt) ? null : student;

            // authentication successful
        }

        public Student GetStudentByUsernameAsync(string username)
        {
            return _context.Students.FirstOrDefault(s => s.Username == username);
        }

        public Student Create(Student user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Students.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            //  check if group exists
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Id = Guid.NewGuid();
            _context.Students.Add(user);
            _context.SaveChanges();

            return user;
        }

        public Student CreateFromTelegram(Student student, long telegramId)
        {
            student.Telegram_Id = telegramId;
            student.Id = Guid.NewGuid();
            _context.Students.Add(student);
            _context.SaveChanges();

            return student;
        }

        public Student GetById(Guid id)
        {
            return _context.Students.Find(id);
        }

       
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        } 
        
        
    }
}