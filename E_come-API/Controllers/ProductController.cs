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

                var result = await work.productRepository.GetAllAsync(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApi(500, ex.Message));
            }
        }

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
                return Conflict(new ResponseApi(409, "Product is referenced by other data and cannot be deleted"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApi(500, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddPDtos dtos)
        {
            try
            {
                if (dtos == null) return BadRequest(new ResponseApi(400));

                var category = await work.categoryRepository.GetByIdAsync(dtos.CategoryId);
                if (category == null)
                    return BadRequest(new ResponseApi(400, "الكاتجري مش موجودة"));

                if (category.HasSizes && (dtos.Sizes == null || dtos.Sizes.Count == 0))
                    return BadRequest(new ResponseApi(400, "لازم تختار مقاس واحد على الأقل للمنتج ده"));

                if (!category.HasSizes)
                    dtos.Sizes = null;

                // ✅ Barcode: تنظيف + فحص التكرار
                dtos.Barcode = string.IsNullOrWhiteSpace(dtos.Barcode) ? null : dtos.Barcode.Trim();
                if (dtos.Barcode != null && await work.productRepository.BarcodeExistsAsync(dtos.Barcode))
                    return Conflict(new ResponseApi(409, "الباركود ده مستخدم لمنتج تاني"));

                await work.productRepository.AddAsync(dtos);
                return Ok(new ResponseApi(201));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] EditProdDto dto)
        {
            if (id != dto.Id) return BadRequest(new ResponseApi(400, "ID mismatch"));

            try
            {
                var category = await work.categoryRepository.GetByIdAsync(dto.CategoryId);
                if (category == null)
                    return BadRequest(new ResponseApi(400, "الكاتجري مش موجودة"));

                if (category.HasSizes && (dto.Sizes == null || dto.Sizes.Count == 0))
                    return BadRequest(new ResponseApi(400, "لازم تختار مقاس واحد على الأقل للمنتج ده"));

                if (!category.HasSizes)
                    dto.Sizes = null;

                // ✅ Barcode: تنظيف + فحص التكرار (مع استثناء المنتج نفسه)
                dto.Barcode = string.IsNullOrWhiteSpace(dto.Barcode) ? null : dto.Barcode.Trim();
                if (dto.Barcode != null && await work.productRepository.BarcodeExistsAsync(dto.Barcode, dto.Id))
                    return Conflict(new ResponseApi(409, "الباركود ده مستخدم لمنتج تاني"));

                var updated = await work.productRepository.UpdateAsync(dto);
                if (!updated) return NotFound(new ResponseApi(404, "Product not found"));

                return Ok(new ResponseApi(200));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500, ex.Message));
            }
        }

        [HttpGet("sizes")]
        public IActionResult GetSizes()
        {
            var sizes = Enum.GetValues<ProductSize>()
                .Select(s => new { Id = (int)s, Name = s.ToString() });
            return Ok(sizes);
        }
    }
}