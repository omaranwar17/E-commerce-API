using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_come_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : BaseController
    {
        public BasketController(IUintOfWork work, IMapper mapper) : base(work, mapper)
        {

        }

        // GET: api/basket/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Basket id is required.");

            var basket = await work.customerBasket.GetBasketAsync(id);

            // لو مفيش سلة، نرجع سلة فاضية بنفس الـ id بدل null
            return Ok(basket ?? new CustomerBasket { Id = id });
        }



        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        {
            if (basket is null || string.IsNullOrWhiteSpace(basket.Id))
                return BadRequest("Basket and basket id are required.");

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

        // POST: api/basket
        //[HttpPost]
        //public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        //{
        //    if (basket is null || string.IsNullOrWhiteSpace(basket.Id))
        //        return BadRequest("Basket and basket id are required.");

        //    var updatedBasket = await work.customerBasket.UpdateBasketAsync(basket);

        //    if (updatedBasket is null)
        //        return BadRequest("Failed to update the basket.");

        //    return Ok(updatedBasket);
        //}

        // DELETE: api/basket/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasket(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Basket id is required.");

            var deleted = await work.customerBasket.Delete(id);

            if (!deleted)
                return NotFound("Basket not found.");

            return NoContent();
        }


    }
}
