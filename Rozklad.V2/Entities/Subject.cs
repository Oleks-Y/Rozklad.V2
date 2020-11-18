using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Rozklad.V2.Entities
{
    public class Subject
    {

        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Groups")]
        public Guid GroupId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Teachers { get; set; }    
      
        public string LessonsZoom { get; set; }
   
        public string LabsZoom { get; set; }

        public string LessonsAccessCode { get; set; }
        
        public string LabsAccessCode { get; set; }

    }
}