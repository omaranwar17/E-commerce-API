using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Services
{
    public interface IImageServices
    {
        Task<List<string>> AddImageAsync(IFormFileCollection formFile, string src);
        void DeleteAsync(string src);
        
    }
}
