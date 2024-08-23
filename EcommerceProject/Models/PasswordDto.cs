using System.ComponentModel.DataAnnotations;

namespace ECommerceProject.Models
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "The Current Password is required"), MaxLength(100)]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "The New Password is required"), MaxLength(100)]
        public string NewPassword { get; set; } = "";


        [Required(ErrorMessage = "The Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "Confirm Password and Password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
