using System.ComponentModel.DataAnnotations;

namespace ECommerceProject.Models
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "The Delivery is required")]
        [MaxLength(300)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
