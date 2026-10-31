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
        private readonly IInvoiceService _invoices;
        private readonly PaymentSettings _payment;
        private readonly IFileService _files;                              // <-- جديد

        public OrdersController(
            IOrderService orders,
            IInvoiceService invoices,
            IOptions<PaymentSettings> payment,
            IFileService files)                                            // <-- جديد
        {
            _orders = orders;
            _invoices = invoices;
            _payment = payment.Value;
            _files = files;                                                // <-- جديد
        }

        [HttpGet("payment-info")]
        public ActionResult<PaymentSettings> GetPaymentInfo() => Ok(_payment);

        [HttpPost]
        [Consumes("multipart/form-data")]                                  // <-- جديد
        public async Task<ActionResult<OrderDto>> Create([FromForm] AddOrderDto dto)   // <-- جديد: FromForm
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? email;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            // <-- جديد: رفع صورة الإيصال لو طريقة الدفع تحتاج تحويل
            string? receiptFileName = null;

            if (PaymentMethods.RequiresReference(dto.PaymentMethod))
            {
                if (dto.ReceiptImage is null)
                    return BadRequest(new { message = "صورة التحويل مطلوبة." });

                try
                {
                    receiptFileName = await _files.SaveReceiptAsync(dto.ReceiptImage);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            var (order, error) = await _orders.CreateAsync(
                userId, email ?? string.Empty, dto.ShippingAddress,
                dto.PaymentMethod, dto.PaymentReference,
                receiptFileName);                                          // <-- جديد

            if (error is not null)
            {
                // <-- جديد: الطلب فشل، فامسح الصورة عشان متتراكمش
                if (receiptFileName is not null)
                    _files.DeleteReceipt(receiptFileName);

                return BadRequest(new { message = error });
            }

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

            var order = User.IsInRole("Admin")
                ? await _orders.GetByIdAsync(id)             // الأدمن يشوف أي طلب
                : await _orders.GetMyOrderAsync(email, id);  // العميل يشوف طلبه بس

            return order is null ? NotFound() : Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // هات اسم الصورة قبل المسح
            var fileName = await _orders.GetReceiptFileNameAsync(id);

            var (success, error, notFound) = await _orders.DeleteAsync(id);
            if (notFound) return NotFound(new { message = error });
            if (!success) return BadRequest(new { message = error });

            // امسح الصورة بعد نجاح المسح من الداتابيز
            if (!string.IsNullOrEmpty(fileName))
                _files.DeleteReceipt(fileName);

            return NoContent();
        }

        [Authorize] // أي حد مسجل دخول
        [HttpGet("{id:int}/my-invoice")]
        public async Task<IActionResult> GetMyInvoice(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var order = await _orders.GetMyOrderAsync(email, id); // بتجيب أوردر العميل بس
            if (order is null || order.InvoiceNumber is null)
                return NotFound(new { message = "الفاتورة غير متاحة." });

            var pdf = _invoices.Generate(order);
            return File(pdf, "application/pdf", $"{order.InvoiceNumber}.pdf");
        }

        [Authorize(Roles = "Admin")] // الأدمن بس اللي يقدر يدخل هنا
        [HttpGet("{id:int}/admin-invoice")]
        public async Task<IActionResult> GetInvoiceForAdmin(int id)
        {
            // الأدمن بيجيب الأوردر بالـ ID مباشرة من غير ما نهتم بالإيميل
            var order = await _orders.GetByIdAsync(id);

            if (order is null)
                return NotFound(new { message = "الطلب غير موجود." });

            if (order.InvoiceNumber is null)
                return BadRequest(new { message = "لم يتم إصدار فاتورة لهذا الطلب بعد." });

            var pdf = _invoices.Generate(order);
            return File(pdf, "application/pdf", $"{order.InvoiceNumber}.pdf");
        }

        // <-- جديد: عرض صورة الإيصال للأدمن بس
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/receipt")]
        public async Task<IActionResult> GetReceipt(int id)
        {
            var fileName = await _orders.GetReceiptFileNameAsync(id);
            if (string.IsNullOrEmpty(fileName))
                return NotFound(new { message = "لا توجد صورة إيصال لهذا الطلب." });

            var path = _files.GetReceiptPath(fileName);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            return PhysicalFile(path, contentType);
        }
    }
}