using E_com_Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class ImageServices : IImageServices
    {
        IFileProvider _Provider;
        public ImageServices(IFileProvider provider)
        {
            _Provider = provider;
        }


        public async Task<List<string>> AddImageAsync(IFormFileCollection formFile, string src)
        {
            var savedPaths = new List<string>();

            // 1. إزالة المسافات من اسم المنتج عشان ميعملش فولدر فيه مسافة (زي Dell )
            string folderName = src.Trim();

            // 2. استخدام Directory.GetCurrentDirectory() بدل المسار النسبي عشان نضمن مكان المشروع الصح
            var imageD = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folderName);

            if (!Directory.Exists(imageD))
            {
                Directory.CreateDirectory(imageD);
            }

            foreach (var item in formFile)
            {
                if (item.Length > 0)
                {
                    var imageName = item.FileName;
                    var imageSrc = $"Images/{folderName}/{imageName}";
                    var root = Path.Combine(imageD, imageName);

                    using (FileStream st = new FileStream(root, FileMode.Create))
                    {
                        await item.CopyToAsync(st);
                    }
                    ;

                    savedPaths.Add(imageSrc);
                }
            }
            return savedPaths;
        }

        //public async Task<List<string>> AddImageAsync(IFormFileCollection formFile, string src)
        //{
        //    var savedPaths = new List<string>();
        //    var imageD = Path.Combine("wwwroot" , "Images", src);
        //    if (Directory.Exists(imageD) is not true)
        //    {
        //        Directory.CreateDirectory(imageD);
        //    }

        //    foreach (var item in formFile)
        //    {
        //        if (item.Length>0)
        //        {
        //            var imageName = item.FileName;
        //            var imageSrc = $"Images/{src}/{imageName}";
        //            var root = Path.Combine(imageD , imageName);
        //            using(FileStream st= new FileStream(root , FileMode.Create)){

        //                await item.CopyToAsync(st);
        //            };
        //            savedPaths.Add(imageSrc);


        //        }
        //    }
        //    return savedPaths;

        //}

        public void DeleteAsync(string src)
        {
            var info = _Provider.GetFileInfo(src);
            var root = info.PhysicalPath;
            File.Delete(root);
        }
    }
}
