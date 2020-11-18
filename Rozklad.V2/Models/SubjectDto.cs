using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rozklad.V2.Models
{
    public class SubjectDto
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Teachers { get; set; }
        public string LessonsZoom { get; set; }
        public string LabsZoom { get; set; }
        public bool IsRequired { get; set; }
        public string LessonsAccessCode { get; set; }
        public string LabsAccessCode { get; set; }
    }
}