using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozklad.V2.Entities{

    /// <summary>
    /// Entity with data about events for student, and info about calendar
    /// </summary>

    public class EventForStudent{

        [Key]
         public Guid Id { get; set; }

         [Required]
         public Guid Student_Id {get; set; }

        public string Event_Id {get; set; }

        public string Calendar_Id {get; set;}

    }
}