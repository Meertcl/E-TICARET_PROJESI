using System.ComponentModel.DataAnnotations;

namespace ECommerceProject.Models
{
    public class LoginDto
    {
        [Required]
        public string EMail { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }
}
