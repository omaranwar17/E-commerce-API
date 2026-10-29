using E_com_Core.Dtos;
using E_com_Core.Interfases;
using E_com_Core.Model.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_come_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/orders")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orders;

        public AdminOrdersController(IOrderService orders) => _orders = orders;

        [HttpGet]
        public async Task<ActionResult<List<AdminOrderListItemDto>>> GetAll(
            [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!string.IsNullOrWhiteSpace(status) && !OrderStatuses.All.Contains(status))
                return BadRequest(new { message = "حالة الطلب غير صحيحة." });

            return Ok(await _orders.GetAdminListAsync(status, page, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _orders.GetByIdAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<OrderDto>> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var (order, error, notFound) = await _orders.UpdateStatusAsync(id, dto.Status);

            if (notFound) return NotFound(new { message = error });
            if (error is not null) return BadRequest(new { message = error });

            return Ok(order);
        }
    }
}