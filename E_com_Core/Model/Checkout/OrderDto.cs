using E_com_Core.Model.Checkout;

namespace E_com_Core.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public bool HasReceipt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public ShippingAddressDto ShippingAddress { get; set; } = new();   // ← الجديد
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceVoidedAt { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public string? Size { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}