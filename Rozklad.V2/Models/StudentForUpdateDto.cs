using System;
using System.Collections.Generic;

namespace Rozklad.V2.Models
{
    public class StudentForUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public IEnumerable<Guid> DisabledSubjects { get; set; } = new List<Guid>();
    }
}