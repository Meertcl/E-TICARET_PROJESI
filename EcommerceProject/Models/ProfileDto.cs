using System.ComponentModel.DataAnnotations;

namespace ECommerceProject.Models
{
    public class ProfileDto
    {
        [Required(ErrorMessage = "The First Name is required"), MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "The Last Name is required"), MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, MaxLength(100)]
        public string EMail { get; set; } = "";

        [Phone(ErrorMessage = "The Phone Number is not valid"), MaxLength(20)]
        public string PhoneNumber { get; set; } = "";
        [Required, MaxLength(200)]
        public string Address { get; set; } = "";
    }
}
