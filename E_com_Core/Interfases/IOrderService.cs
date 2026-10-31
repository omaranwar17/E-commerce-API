using E_com_Core.Dtos;
using E_com_Core.Model.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Interfases
{
    public interface IOrderService
    {
        Task<(OrderDto? Order, string? Error)> CreateAsync(
            string userId, string email, ShippingAddressDto address,
            string paymentMethod, string? paymentReference,
            string? receiptImage);                                   // <-- جديد

        Task<List<OrderDto>> GetMyOrdersAsync(string email, int page, int pageSize);
        Task<OrderDto?> GetMyOrderAsync(string email, int id);

        Task<List<AdminOrderListItemDto>> GetAdminListAsync(string? status, int page, int pageSize);
        Task<OrderDto?> GetByIdAsync(int id);
        Task<(OrderDto? Order, string? Error, bool NotFound)> UpdateStatusAsync(int id, string newStatus);

        Task<string?> GetReceiptFileNameAsync(int id);
        Task<(bool Success, string? Error, bool NotFound)> DeleteAsync(int id);
    }
}