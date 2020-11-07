using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozklad.V2.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        public string Group_Name { get; set; }
        
        
        public IEnumerable<Subject> Subjects { get; set; }
    }
}