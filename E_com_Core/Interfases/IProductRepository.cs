using E_com_Core.Model.Common;
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
        //Task<IEnumerable<ProduDtoP>> GetAllAsync(string sort , int categoryId);
        // شيل الـ Tuple وخليها ترجع PagedResult
        Task<PagedResult<ProduDtoP>> GetAllAsync(ProductQueryParams query);
        Task<bool> AddAsync(AddPDtos productDto);
        Task<bool> UpdateAsync(EditProdDto productDto);
        Task<bool> Delete(Product product);
    }
}
