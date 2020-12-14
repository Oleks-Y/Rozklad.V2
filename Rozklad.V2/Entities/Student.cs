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
        
        /// <summary>
        /// When users authentificate via pasword, we need this, but when user registered via telegram, this field will be null 
        /// </summary>
        public byte[] PasswordHash { get; set; }
        /// <summary>
        /// When users authentificate via pasword, we need this, but when user registered via telegram, this field will be null 
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        public IEnumerable<DisabledSubject> DisabledSubjects { get; set; }
        public IEnumerable<MutedSubject> MutedSubjects { get; set; }
        
        public long? Telegram_Id { get; set; }
        
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
        
        public NotificationsSettings NotificationsSettings { get; set; }
        
        [ForeignKey("NotificationsSettings")]
        public Guid? NotificationsSettingsId { get; set; }

        // [NotMapped] public IEnumerable<string> Subjects { get; set; } 
    }
}