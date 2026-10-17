using AutoMapper;
using E_com_Core.Interfases;
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

        public async Task<bool> AddAsync(AddPDtos productDto)
        {
            if (productDto == null) return false;
            
            var prod = mapper.Map<Product>(productDto);
           await storeDb.Products.AddAsync(prod);
            await storeDb.SaveChangesAsync();
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

        public async Task<bool> UpdateAsync(EditProdDto productDto)
        {
            if (productDto == null) return false;
            var findProd =  storeDb.Products.Include((p) => p.Categorys)
                 .Include((p) => p.Photos).FirstOrDefault((p) => p.Id == productDto.Id);
            if (findProd == null) return false;
           mapper.Map(productDto , findProd);
            var findId = await storeDb.Photos.Where((p) => p.ProductId == productDto.Id).ToListAsync();
            foreach (var item in findId)
            {
                services.DeleteAsync(item.ImageName);

            }
            storeDb.Photos.RemoveRange(findId);
            var addImg = await services.AddImageAsync(productDto.Photos , productDto.Name);
            var path = addImg.Select((s) => new Photo
            {
                ImageName = s,
                ProductId = productDto.Id,
            }).ToList();
            await storeDb.Photos.AddRangeAsync(path);
            await storeDb.SaveChangesAsync();
            return true;
        }
    }
}
