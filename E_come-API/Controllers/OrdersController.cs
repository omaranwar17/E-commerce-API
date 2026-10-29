using E_com_Core.Dtos;
using E_com_Core.Interfases;
using E_com_Core.Model.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace E_come_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orders;
        private readonly PaymentSettings _payment;

        public OrdersController(IOrderService orders, IOptions<PaymentSettings> payment)
        {
            _orders = orders;
            _payment = payment.Value;
        }

        [HttpGet("payment-info")]
        public ActionResult<PaymentSettings> GetPaymentInfo() => Ok(_payment);

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create([FromBody] AddOrderDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? email;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var (order, error) = await _orders.CreateAsync(
                userId, email ?? string.Empty, dto.ShippingAddress,
                dto.PaymentMethod, dto.PaymentReference);

            if (error is not null)
                return BadRequest(new { message = error });

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetMine(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            return Ok(await _orders.GetMyOrdersAsync(email, page, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetMineById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var order = await _orders.GetMyOrderAsync(email, id);
            return order is null ? NotFound() : Ok(order);
        }
    }
}