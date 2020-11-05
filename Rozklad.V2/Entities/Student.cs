using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rozklad.V2.Entities
{
    public class Student
    {
        [Key] public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string Group { get; set; }

        [NotMapped]
        public IEnumerable<string> Subjects { get; set; }
    }
}