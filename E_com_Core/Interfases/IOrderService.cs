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
            string paymentMethod, string? paymentReference);
    }
}
