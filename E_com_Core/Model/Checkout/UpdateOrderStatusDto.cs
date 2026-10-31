using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    public class AdminOrderListItemDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string BuyerEmail { get; set; } = string.Empty;
        public bool HasReceipt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public decimal Total { get; set; }
    }
}
