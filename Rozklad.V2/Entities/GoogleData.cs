using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hangfire.MemoryStorage.Utilities;

namespace Rozklad.V2.Entities
{
    public class GoogleData
    {
        [Key] public Guid Id { get; set; }
        
        [ForeignKey("Students")]
        public Guid StudentId { get; set; }
        
        public string CalendarId { get; set; }
        
        public string Email { get; set; }

        public string RefreshToken { get; set; }
    }
}