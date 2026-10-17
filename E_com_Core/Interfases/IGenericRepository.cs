using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace E_com_Core.Interfases
{
    public interface IGenericRepository<T> where T : class
    {
       public  Task<IReadOnlyList<T>> GetAllAsync();
       public  Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] inclodes);
       public  Task<T> GetByIdAsync(int id);
       public  Task<T> GetByIdAsync(int id , params Expression<Func<T, object>>[] inclodes);
       public Task Update(T entity);
       public  Task AddAsync(T entity);
       public Task Delete(int id);
     
    }
}
