using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozklad.V2.Entities
{
    /// <summary>
    /// Entity with data about time of lesson, and what discipline it`s
    /// </summary>
    public class Lesson 
    {    
        [Key]
        public Guid Id { get; set; }
        
        // [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        [ForeignKey("Subjects")]
        public Guid SubjectId { get; set; }
        
        public Subject Subject { get; set; }
        
        [Required]
        public int Week { get; set; }
        
        [Required]
        public int DayOfWeek { get; set; }
        
        [Required]
        public string TimeStart { get; set; }
         
        public string Type { get; set; }
        
        // public Subject Subject { get; set; }

    }

    
    
}