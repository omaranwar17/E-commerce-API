using E_com_Core.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace E_com_Core.Interfases
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<bool> AddAsync(AddPDtos productDto);
        Task<bool> UpdateAsync(EditProdDto productDto);
        Task<bool> Delete(Product product);
    }
}
