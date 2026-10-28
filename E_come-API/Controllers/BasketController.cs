using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_come_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : BaseController
    {
        public BasketController(IUintOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        // الـ key بيتحدد من التوكن، مش من الـ client
        private string? BasketKey
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue(ClaimTypes.Email);
                return string.IsNullOrWhiteSpace(userId) ? null : $"basket:{userId}";
            }
        }

        // GET: api/basket
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket()
        {
            var key = BasketKey;
            if (key is null) return Unauthorized();

            var basket = await work.customerBasket.GetBasketAsync(key);

            return Ok(basket ?? new CustomerBasket { Id = key });
        }

        // POST: api/basket
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        {
            var key = BasketKey;
            if (key is null) return Unauthorized();

            if (basket is null || basket.basketItems is null)
                return BadRequest("Basket is required.");

            // نتجاهل أي Id جاي من الـ client
            basket.Id = key;

            foreach (var item in basket.basketItems)
            {
                if (item.Qunatity < 1)
                    return BadRequest("Invalid quantity.");

                var product = await work.productRepository.GetByIdAsync(item.Id);
                if (product is null)
                    return BadRequest($"Product {item.Id} not found.");

                // ما نثقش في اللي جاي من الـ client
                item.Price = product.NewPrice;
                item.Name = product.Name;
            }

            var updatedBasket = await work.customerBasket.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return BadRequest("Failed to update the basket.");

            return Ok(updatedBasket);
        }

        // DELETE: api/basket
        [HttpDelete]
        public async Task<IActionResult> DeleteBasket()
        {
            var key = BasketKey;
            if (key is null) return Unauthorized();

            var deleted = await work.customerBasket.Delete(key);

            if (!deleted)
                return NotFound("Basket not found.");

            return NoContent();
        }
    }
}