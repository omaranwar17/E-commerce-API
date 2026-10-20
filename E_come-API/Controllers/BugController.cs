using AutoMapper;
using E_com_Core.Interfases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_come_API.Controllers
{
   
    public class BugController : BaseController
    {
        public BugController(IUintOfWork work, IMapper mapper) : base(work, mapper)
        {
  
        }

        [HttpGet("NotFound")]
        public async Task<IActionResult> GetNotFound()
        {
            var categ = await work.categoryRepository.GetByIdAsync(100);
            if (categ == null) return NotFound();

            return  Ok(categ);  
        }


        [HttpGet("ServerError")]
        public async Task<IActionResult> GetServerError()
        {
            var categ = await work.categoryRepository.GetByIdAsync(100);
            categ.Name = "";
            return Ok(categ);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBadRequest(int id)
        {

            return Ok();
        }
        [HttpGet("BadRequest")]
        public async Task<IActionResult> GetBadRequest()
        {

            return BadRequest();
        }
    }
}
