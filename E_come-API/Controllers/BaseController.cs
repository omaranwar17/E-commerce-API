using AutoMapper;
using E_com_Core.Interfases;
using E_come_API.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_come_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
       public readonly IUintOfWork work;
       public readonly IMapper mapper;
        public BaseController(IUintOfWork work , IMapper mapper)
        {
            this.work = work;
            this.mapper = mapper;
        }
    }
}
