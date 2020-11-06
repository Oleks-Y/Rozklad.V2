using System;
using System.ComponentModel.DataAnnotations;

namespace Rozklad.V2.Models
{
    public class AuthentificateDto
    {
        public Guid Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Group { get; set; }

        public string Token { get; set; }
    }
}