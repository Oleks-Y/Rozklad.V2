using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozklad.V2.Entities
{
    public class DisabledSubject
    {
        [Key] public Guid Id { get; set; }
        [ForeignKey("Subjects")]
        public Guid SubjectId { get; set; }
        [ForeignKey("Students")]        
        public Guid StudentId { get; set; }
    }
}