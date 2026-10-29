using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public string PaymentMethod { get; set; } = "CashOnDelivery";
        public List<OrderItem> Items { get; set; } = new();
      
        public decimal Subtotal { get; set; }
        public string? PaymentReference { get; set; }
        public decimal ShippingFee { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = new();
        public decimal Total => Subtotal + ShippingFee;
    }
}
