using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozklad.V2.Entities
{
    /// <summary>
    /// this subjects only muted for notifications 
    /// </summary>
    public class MutedSubject
    {
        [Key] public Guid Id { get; set; }
        [ForeignKey("Subjects")] public Guid SubjectId { get; set; }
        [ForeignKey("Students")] public Guid StudentId { get; set; }
    }
}