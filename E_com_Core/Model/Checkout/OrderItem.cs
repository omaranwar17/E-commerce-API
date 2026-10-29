using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public string? Size { get; set; }          // ← الجديد
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
