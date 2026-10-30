

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

        private string? BasketKey
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue(ClaimTypes.Email);
                return string.IsNullOrWhiteSpace(userId) ? null : $"basket:{userId}";
            }
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket()
        {
            var key = BasketKey;
            if (key is null) return Unauthorized();

            var basket = await work.customerBasket.GetBasketAsync(key);

            return Ok(basket ?? new CustomerBasket { Id = key });
        }

        //[HttpPost]
        //public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        //{
        //    var key = BasketKey;
        //    if (key is null) return Unauthorized();

        //    if (basket is null || basket.basketItems is null)
        //        return BadRequest("Basket is required.");

        //    basket.Id = key;

        //    foreach (var item in basket.basketItems)
        //    {
        //        if (item.Qunatity < 1)
        //            return BadRequest("Invalid quantity.");

        //        var product = await work.productRepository.GetByIdAsync(item.Id);
        //        if (product is null)
        //            return BadRequest($"Product {item.Id} not found.");

        //        // ✅ التعديل هنا: بنحدث السعر بس، وسيبنا الاسم والمقاس زي ما هما جايين من الفرونت
        //        item.Price = product.NewPrice;
        //    }

        //    var updatedBasket = await work.customerBasket.UpdateBasketAsync(basket);

        //    if (updatedBasket is null)
        //        return BadRequest("Failed to update the basket.");

        //    return Ok(updatedBasket);
        //}


        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        {
            var key = BasketKey;
            if (key is null) return Unauthorized();

            if (basket is null || basket.basketItems is null)
                return BadRequest("Basket is required.");

            basket.Id = key;

            var validItems = new List<BasketItem>();
            foreach (var item in basket.basketItems)
            {
                if (item.Qunatity < 1)
                    return BadRequest("Invalid quantity.");

                var product = await work.productRepository.GetByIdAsync(item.Id);
                if (product is null) continue; // منتج اتحذف: نتجاهله بدل ما نفشل الطلب كله

                item.Price = product.NewPrice;
                validItems.Add(item);
            }
            basket.basketItems = validItems;

            var updatedBasket = await work.customerBasket.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return BadRequest("Failed to update the basket.");

            return Ok(updatedBasket);
        }

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