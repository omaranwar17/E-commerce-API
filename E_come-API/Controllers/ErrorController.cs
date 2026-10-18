using E_come_API.Helpar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_come_API.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiController]
    public class ErrorController : ControllerBase
    {

        [HttpGet]
        public IActionResult Error(int statusCode)
        {
            return new ObjectResult(new ResponseApi(statusCode)); 
        }
    }
}
