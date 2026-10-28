using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model.Products;
using E_com_Infrastructurer.Repository;
using E_come_API.DTO;
using E_come_API.Helpar;
using E_come_API.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_come_API.Controllers
{
   
    public class CategoryController : BaseController
    {
        
        public CategoryController(IUintOfWork work , IMapper mapper) : base(work , mapper)
        {
         
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var cat = await work.categoryRepository.GetAllAsync();
                if (cat == null) return BadRequest(new ResponseApi(400));
                return Ok(cat);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500));
            }

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCatId(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new ResponseApi(400));
                var catId = await work.categoryRepository.GetByIdAsync(id);
                if (catId == null) return BadRequest(new ResponseApi(400));
                return Ok(new EditCateDto
                {
                    Id = catId.Id,
                    Name = catId.Name,
                    Description = catId.Description
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseApi(500));
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new ResponseApi(400));

                await work.categoryRepository.Delete(id);

                return Ok(new ResponseApi(200));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDto category)
        {
            try
            {
                var add = mapper.Map<Category>(category);
                if (add == null) return BadRequest(new ResponseApi(400));
                await work.categoryRepository.AddAsync(add);
                return Ok(new ResponseApi(201)); 

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseApi(500));
            }
        }

 
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] EditCateDto category)
        {
            try
            {
                if (id <= 0 || category is null || id != category.Id)
                    return BadRequest(new ResponseApi(400));

                var edit = mapper.Map<Category>(category);
                await work.categoryRepository.Update(edit);
                return Ok(new ResponseApi(200));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApi(500));
            }
        }



    }
}
