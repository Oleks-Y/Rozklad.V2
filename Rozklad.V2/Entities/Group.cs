using System;
using System.ComponentModel.DataAnnotations;

namespace Rozklad.V2.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        public string Group_Name { get; set; }
    }
}