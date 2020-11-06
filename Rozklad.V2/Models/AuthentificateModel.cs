using System.ComponentModel.DataAnnotations;

namespace Rozklad.V2.Models
{
    public class AuthentificateModel
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}