using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Rozklad.V2.Entities
{
    public class Student
    {
        [Key] public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public Guid GroupId { get; set; }
        
        public Group Group { get; set; }
        
        public string Username { get; set; }
        
        
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public IEnumerable<DisabledSubject> DisabledSubjects { get; set; }
        
        public long? Telegram_Id { get; set; }
        
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
        // [NotMapped] public IEnumerable<string> Subjects { get; set; } 
    }
}