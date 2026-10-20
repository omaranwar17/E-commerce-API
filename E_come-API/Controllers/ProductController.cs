using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model.Products;
using E_come_API.DTO;
using E_come_API.Helpar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new ResponseApi(400));

              var prodId = await work.productRepository.GetByIdAsync(id, (p)=>p.Photos, (p)=>p.Categorys);
                await work.productRepository.Delete(prodId);
                return Ok(new ResponseApi(200));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500));
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(AddPDtos  dtos)
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

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(EditProdDto dto)
        {
            try
            {
                var edit = mapper.Map<Product>(dto);
                if (edit == null) return BadRequest(new ResponseApi(400));
                await work.productRepository.Update(edit);
                return Ok(new ResponseApi(200));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500, ex.Message));
            }
        }
    }
}
