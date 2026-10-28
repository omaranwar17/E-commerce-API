using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model.Products;
using E_come_API.DTO;
using E_come_API.Helpar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_come_API.Controllers
{

    public class ProductController : BaseController
    {
        public ProductController(IUintOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromQuery] ProductQueryParams query)
        {
            try
            {
                if (query.PageNumber < 1) query.PageNumber = 1;
                if (query.PageSize < 1 || query.PageSize > 100) query.PageSize = 10;

                // ✅ بنستقبل PagedResult مش Tuple
                var result = await work.productRepository.GetAllAsync(query);

                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApi(500, ex.Message));
            }
        }



        //[HttpGet]
        //public async Task<IActionResult> GetAllProduct()
        //{
        //    try
        //    {
        //        var products = await work.productRepository.GetAllAsync(
        //     s => s.Categorys,
        //     s => s.Photos
        // );
        //        if (products == null) return BadRequest(new ResponseApi(400));
        //        var dto = mapper.Map<List<ProdDto>>(products);

        //        return Ok(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseApi(500, ex.Message));
        //    }

        //}

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductId(int id)
        {
            try
            {
                var proId = await work.productRepository.GetByIdAsync(id,
                    s => s.Categorys,
                    s => s.Photos); 
                var dto = mapper.Map<ProdDto>(proId);
                if (proId == null) return BadRequest(new ResponseApi(400));
                return Ok(dto);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500, ex.Message));
            }

        }

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        if (id <= 0) return BadRequest(new ResponseApi(400));

        //      var prodId = await work.productRepository.GetByIdAsync(id, (p)=>p.Photos, (p)=>p.Categorys);
        //        await work.productRepository.Delete(prodId);
        //        return Ok(new ResponseApi(200));

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseApi(500));
        //    }
        //}

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest(new ResponseApi(400, "Invalid id"));

            try
            {
                var product = await work.productRepository.GetByIdAsync(id, p => p.Photos);
                if (product == null) return NotFound(new ResponseApi(404, "Product not found"));

                var deleted = await work.productRepository.Delete(product);
                if (!deleted) return BadRequest(new ResponseApi(400, "Delete failed"));

                return Ok(new ResponseApi(200));
            }
            catch (DbUpdateException)
            {
                // غالباً المنتج مرتبط ببيانات تانية (طلبات / سلة)
                return Conflict(new ResponseApi(409, "Product is referenced by other data and cannot be deleted"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApi(500, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct( [FromForm] AddPDtos  dtos)
        {
            try
            {
                //var add = mapper.Map<Product>(product);
                if (dtos == null) return BadRequest(new ResponseApi(400));
                await work.productRepository.AddAsync(dtos);
                return Ok(new ResponseApi(201));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500, ex.Message));
            }
        }

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> UpdateProduct(int id,[FromForm] EditProdDto dto)
        //{
        //    if (id != dto.Id) return BadRequest("ID mismatch");

        //    try
        //    {
        //        var edit = mapper.Map<Product>(dto);
        //        if (edit == null) return BadRequest(new ResponseApi(400));
        //        await work.productRepository.Update(edit);
        //        return Ok(new ResponseApi(200));

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseApi(500, ex.Message));
        //    }
        //}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] EditProdDto dto)
        {
            if (id != dto.Id) return BadRequest(new ResponseApi(400, "ID mismatch"));

            try
            {
                var updated = await work.productRepository.UpdateAsync(dto);
                if (!updated) return NotFound(new ResponseApi(404, "Product not found"));

                return Ok(new ResponseApi(200));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500, ex.Message));
            }
        }
    }
}
