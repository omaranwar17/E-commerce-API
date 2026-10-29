using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public class CreateOrderDto
    {
        [Required]
        public ShippingAddressDto ShippingAddress { get; set; } = new();
    }



    public class ShippingAddressDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم موبايل مصري غير صحيح")]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Governorate { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Street { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
