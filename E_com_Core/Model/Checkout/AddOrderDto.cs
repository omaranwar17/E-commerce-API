using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public class AddOrderDto
    {
        [Required]
        public ShippingAddressDto ShippingAddress { get; set; } = new();

        [Required]
        public string PaymentMethod { get; set; } = "CashOnDelivery";

        [MaxLength(100)]
        public string? PaymentReference { get; set; }
        public IFormFile? ReceiptImage { get; set; }
    }
}
