using E_com_Core.Interfases;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxSize = 5 * 1024 * 1024; // 5 ميجا

        public FileService(IWebHostEnvironment env) => _env = env;

        private string ReceiptsFolder =>
            Path.Combine(_env.ContentRootPath, "PrivateUploads", "receipts");

        public async Task<string> SaveReceiptAsync(IFormFile file)
        {
            if (file.Length == 0 || file.Length > MaxSize)
                throw new InvalidOperationException("حجم الصورة غير مسموح (الحد الأقصى 5 ميجا).");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExt.Contains(ext))
                throw new InvalidOperationException("نوع الملف غير مدعوم.");

            Directory.CreateDirectory(ReceiptsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            await using var stream = new FileStream(
                Path.Combine(ReceiptsFolder, fileName), FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        public string GetReceiptPath(string fileName) =>
            Path.Combine(ReceiptsFolder, Path.GetFileName(fileName));

        public void DeleteReceipt(string fileName)
        {
            var path = GetReceiptPath(fileName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
