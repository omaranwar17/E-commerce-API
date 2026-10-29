using E_com_Core.Dtos;
using E_com_Core.Interfases;
using E_com_Core.Model.Checkout;
using E_com_Infrastructurer.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class OrderService : IOrderService
    {
        private const decimal FreeShippingThreshold = 30000; // نفس قيمة الفرونت
        private const decimal ShippingFee = 50;
        private const int MaxQuantityPerItem = 50; // عدّلها حسب سياستك
        private const string DuplicateReferenceError = "رقم العملية مستخدم من قبل.";

        private static readonly Regex PhoneRegex =
            new(@"^01[0125][0-9]{8}$", RegexOptions.Compiled);

        private readonly IUintOfWork _work;
        private readonly StoreDbContext _storeDb;

        public OrderService(IUintOfWork work, StoreDbContext storeDb)
        {
            _work = work;
            _storeDb = storeDb;
        }

        public async Task<(OrderDto? Order, string? Error)> CreateAsync(
            string userId, string email, ShippingAddressDto address,
            string paymentMethod, string? paymentReference)
        {
            // 0) تحقق من طريقة الدفع ورقم العملية
            if (!PaymentMethods.All.Contains(paymentMethod))
                return (null, "طريقة الدفع غير صحيحة.");

            var reference = paymentReference?.Trim();
            var needsReference = PaymentMethods.RequiresReference(paymentMethod);

            if (needsReference)
            {
                if (string.IsNullOrWhiteSpace(reference))
                    return (null, "رقم العملية مطلوب.");

                if (reference.Length > 100)
                    return (null, "رقم العملية طويل جدًا.");

                if (await _storeDb.Orders.AnyAsync(o => o.PaymentReference == reference))
                    return (null, DuplicateReferenceError);
            }

            // 0.5) تحقق من بيانات الشحن
            var (shipping, addressError) = BuildShippingAddress(address);
            if (shipping is null)
                return (null, addressError);

            // 1) هات السلة (نفس الـ key اللي في BasketController)
            var basketKey = $"basket:{userId}";
            var basket = await _work.customerBasket.GetBasketAsync(basketKey);
            if (basket is null || basket.basketItems.Count == 0)
                return (null, "السلة فاضية.");

            var order = new Order
            {
                BuyerEmail = email,
                PaymentMethod = paymentMethod,
                PaymentReference = needsReference ? reference : null,
                Status = needsReference
                    ? OrderStatuses.AwaitingPaymentReview
                    : OrderStatuses.Pending,
                ShippingAddress = shipping
            };

            // 2) حوّل كل عنصر في السلة لـ OrderItem بسعر من الداتابيز
            foreach (var item in basket.basketItems)
            {
                if (item.Qunatity < 1 || item.Qunatity > MaxQuantityPerItem)
                    return (null, "كمية غير صحيحة.");

                var product = await _work.productRepository.GetByIdAsync(item.Id);
                if (product is null)
                    return (null, $"المنتج {item.Id} مش موجود.");

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,    // من الداتابيز
                    UnitPrice = product.NewPrice,  // من الداتابيز
                    PictureUrl = item.Image,
                    Size = item.Size,
                    Quantity = item.Qunatity
                });
            }

            // 3) احسب الإجمالي
            order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            order.ShippingFee = order.Subtotal > FreeShippingThreshold ? 0 : ShippingFee;

            // 4) احفظ
            _storeDb.Orders.Add(order);
            try
            {
                await _storeDb.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                needsReference &&
                ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                // طلبين بنفس رقم العملية في نفس اللحظة: الـ unique index هو اللي بيحسم
                return (null, DuplicateReferenceError);
            }

            // 5) امسح السلة بعد نجاح الحفظ
            try
            {
                await _work.customerBasket.Delete(basketKey);
            }
            catch
            {
                // الأوردر اتحفظ فعلاً، فمش هنفشل الطلب بسبب مسح السلة
            }

            return (ToDto(order), null);
        }

        /// <summary>يتحقق من بيانات الشحن وينضفها (trim) قبل الحفظ.</summary>
        private static (ShippingAddress? Address, string? Error) BuildShippingAddress(ShippingAddressDto? a)
        {
            if (a is null)
                return (null, "بيانات الشحن مطلوبة.");

            var fullName = a.FullName?.Trim();
            var phone = a.Phone?.Trim();
            var governorate = a.Governorate?.Trim();
            var city = a.City?.Trim();
            var street = a.Street?.Trim();
            var notes = string.IsNullOrWhiteSpace(a.Notes) ? null : a.Notes.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(governorate) ||
                string.IsNullOrEmpty(city) || string.IsNullOrEmpty(street))
                return (null, "بيانات الشحن غير مكتملة.");

            if (phone is null || !PhoneRegex.IsMatch(phone))
                return (null, "رقم الموبايل غير صحيح.");

            if (fullName.Length > 100 || governorate.Length > 50 || city.Length > 100 ||
                street.Length > 250 || (notes?.Length ?? 0) > 500)
                return (null, "بيانات الشحن تتجاوز الحد المسموح.");

            return (new ShippingAddress
            {
                FullName = fullName,
                Phone = phone,
                Governorate = governorate,
                City = city,
                Street = street,
                Notes = notes
            }, null);
        }

        private static OrderDto ToDto(Order o) => new()
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            Status = o.Status,
            PaymentMethod = o.PaymentMethod,
            PaymentReference = o.PaymentReference,
            ShippingAddress = new ShippingAddressDto
            {
                FullName = o.ShippingAddress.FullName,
                Phone = o.ShippingAddress.Phone,
                Governorate = o.ShippingAddress.Governorate,
                City = o.ShippingAddress.City,
                Street = o.ShippingAddress.Street,
                Notes = o.ShippingAddress.Notes
            },
            Subtotal = o.Subtotal,
            ShippingFee = o.ShippingFee,
            Total = o.Total,
            Items = o.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                PictureUrl = i.PictureUrl,
                Size = i.Size,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}