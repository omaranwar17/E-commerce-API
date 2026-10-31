using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Interfases
{
    public interface IFileService
    {
        Task<string> SaveReceiptAsync(IFormFile file);
        string GetReceiptPath(string fileName);
        void DeleteReceipt(string fileName);
    }
}
