using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Services;
using E_com_Infrastructurer.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;     
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly StoreDbContext _storeDb;
       

        public GenericRepository(StoreDbContext storeDb 
                           )
        {
            _storeDb = storeDb;
            
        }

        public async Task AddAsync(T entity)
        {
            await _storeDb.Set<T>().AddAsync(entity);
            await _storeDb.SaveChangesAsync();       // ← ;
        }

        public async Task Delete(int id)
        {
            var delete = await _storeDb.Set<T>().FindAsync(id);
            _storeDb.Set<T>().Remove(delete);
            await _storeDb.SaveChangesAsync();       // ← ;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _storeDb.Set<T>().AsNoTracking().ToListAsync();  // ← T كبيرة

        public async Task<IReadOnlyList<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes)            // ← Expression
        {
            var data = _storeDb.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                data = data.Include(item);           // ← ;
            }
            return await data.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var dataId = await _storeDb.Set<T>().FindAsync(id);
            return dataId;
        }

        public async Task<T> GetByIdAsync(
            int id,
            params Expression<Func<T, object>>[] includes)            // ← Expression + async
        {
            var data = _storeDb.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                data = data.Include(item);           // ← ;
            }

            var entity = await data.FirstOrDefaultAsync(
                d => EF.Property<int>(d, "Id") == id);                 // ← FirstOrDefaultAsync

            return entity;
        }

        public async Task Update(T entity)
        {
            _storeDb.Set<T>().Update(entity);        // ← شيل await (Update مش async)
            await _storeDb.SaveChangesAsync();
        }
    }
}