using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model.Common;
using E_com_Core.Model.Products;
using E_com_Core.Services;
using E_com_Infrastructurer.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IMapper mapper;
        private readonly IImageServices services;
        private readonly StoreDbContext storeDb;

        public ProductRepository(StoreDbContext storeDb , IImageServices services, IMapper mapper) : base(storeDb)
        {
            this.mapper = mapper;
   
            this.services = services;
            this.storeDb = storeDb;

        }

        public async Task<PagedResult<ProduDtoP>> GetAllAsync(ProductQueryParams q)
        {
            var query = storeDb.Products
                .AsNoTracking()
                .Include(p => p.Categorys)
                .Include(p => p.Photos)
                .AsQueryable();

           

            // 1. Filter by Category
            if (q.CategoryId > 0)
                query = query.Where(p => p.CategoryId == q.CategoryId);

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                // 1. تحويل النص لحروف صغيرة وتقسيمه لكلمات منفصلة
                var keywords = q.Search.Trim().ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // 2. البحث عن كل كلمة في الاسم أو الوصف
                foreach (var word in keywords)
                {
                    var term = word; // متغير محلي للاستخدام داخل الـ LINQ
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(term) ||
                        p.Description.ToLower().Contains(term));
                }
            }

            // 3. Count (هيتحسب بعد الفلاتر كلها)
            var totalCount = await query.CountAsync();

            query = q.Sort?.ToLower() switch
            {
                "price" => query.OrderBy(p => p.NewPrice).ThenBy(p => p.Id),
                "price_desc" => query.OrderByDescending(p => p.NewPrice).ThenBy(p => p.Id),
                "name" => query.OrderBy(p => p.Name).ThenBy(p => p.Id),
                "newest" => query.OrderByDescending(p => p.Id),
                _ => query.OrderBy(p => p.Id)
            };

            var data = await query
                .Skip((q.PageNumber - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(p => new ProduDtoP
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    NewPrice = p.NewPrice,
                    OldPrice = p.OldPrice,
                    Barcode = p.Barcode,
                    CategoryName = p.Categorys.Name,
                    Photos = p.Photos.Select(ph => new PhotoDtos
                    {
                        ProductId = ph.ProductId,
                        ImageName = ph.ImageName
                    }).ToList()
                })
                .ToListAsync();

            // ✅ هنا بنرجع الشكل اللي في Postman
            return new PagedResult<ProduDtoP>
            {
                PageNumber = q.PageNumber,
                PageSize = q.PageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        public async Task<bool> AddAsync(AddPDtos productDto)
        {
            if (productDto == null) return false;
            
            var prod = mapper.Map<Product>(productDto);
           await storeDb.Products.AddAsync(prod);
            await storeDb.SaveChangesAsync();
            if (string.IsNullOrEmpty(prod.Barcode))
                prod.Barcode = BarcodeGenerator.GenerateEan13(prod.Id);
            var img = await services.AddImageAsync(productDto.Photos , productDto.Name);
            var path = img.Select((s) => new Photo
            {
                ImageName = s,
                ProductId = prod.Id,
            }).ToList();
            await storeDb.Photos.AddRangeAsync(path);
            await storeDb.SaveChangesAsync();
            return true;


        }

        public async Task<bool> Delete(Product product)
        {
            if (product == null) return false;
            var prod = await storeDb.Photos.Where((p) => p.ProductId == product.Id).ToListAsync();
            foreach (var  item in prod)
            {
                services.DeleteAsync(item.ImageName);

            }
            storeDb.Products.Remove(product);
            await storeDb.SaveChangesAsync();
            return true;
        }

        //public async Task<bool> UpdateAsync(EditProdDto productDto)
        //{
        //    if (productDto == null) return false;
        //    var findProd =  storeDb.Products.Include((p) => p.Categorys)
        //         .Include((p) => p.Photos).FirstOrDefault((p) => p.Id == productDto.Id);
        //    if (findProd == null) return false;
        //   mapper.Map(productDto , findProd);
        //    var findId = await storeDb.Photos.Where((p) => p.ProductId == productDto.Id).ToListAsync();
        //    foreach (var item in findId)
        //    {
        //        services.DeleteAsync(item.ImageName);

        //    }
        //    storeDb.Photos.RemoveRange(findId);
        //    var addImg = await services.AddImageAsync(productDto.Photos , productDto.Name);
        //    var path = addImg.Select((s) => new Photo
        //    {
        //        ImageName = s,
        //        ProductId = productDto.Id,
        //    }).ToList();
        //    await storeDb.Photos.AddRangeAsync(path);
        //    await storeDb.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> UpdateAsync(EditProdDto productDto)
        {
            if (productDto == null) return false;

            var findProd = storeDb.Products.Include((p) => p.Categorys)
                 .Include((p) => p.Photos).FirstOrDefault((p) => p.Id == productDto.Id);

            if (findProd == null) return false;

            // 1. حفظ الباركود الحالي قبل الـ Map عشان الـ Map بيكتب فوقه بـ null
            var currentBarcode = findProd.Barcode;

            // 2. تحديث بيانات المنتج الأساسية
            mapper.Map(productDto, findProd);

            // 3. الباركود: الجديد لو اتبعت، وإلا القديم، وإلا يتولد تلقائي
            findProd.Barcode = !string.IsNullOrWhiteSpace(productDto.Barcode)
                ? productDto.Barcode.Trim()
                : (string.IsNullOrEmpty(currentBarcode)
                    ? BarcodeGenerator.GenerateEan13(findProd.Id)
                    : currentBarcode);

            // 4. التعامل مع الصور
            if (productDto.Photos != null && productDto.Photos.Count > 0)
            {
                var oldPhotos = await storeDb.Photos.Where((p) => p.ProductId == productDto.Id).ToListAsync();
                foreach (var item in oldPhotos)
                {
                    services.DeleteAsync(item.ImageName);
                }
                storeDb.Photos.RemoveRange(oldPhotos);

                var addImg = await services.AddImageAsync(productDto.Photos, productDto.Name);
                var path = addImg.Select((s) => new Photo
                {
                    ImageName = s,
                    ProductId = productDto.Id,
                }).ToList();

                await storeDb.Photos.AddRangeAsync(path);
            }
            // لو مفيش صور جديدة، الصور القديمة تفضل زي ما هي

            await storeDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null)
        {
            return await storeDb.Products
                .AnyAsync(p => p.Barcode == barcode && (excludeId == null || p.Id != excludeId));
        }
    }
}
