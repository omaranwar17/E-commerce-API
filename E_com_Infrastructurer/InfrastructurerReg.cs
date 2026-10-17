using E_com_Core.Interfases;
using E_com_Core.Services;
using E_com_Infrastructurer.Data.Context;
using E_com_Infrastructurer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_com_Infrastructurer
{
    public static class InfrastructurerReg
    {
        public static IServiceCollection InfrastructurerColl(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. DbContext
            services.AddDbContext<StoreDbContext>((op) =>
            {
                op.UseSqlServer(configuration.GetConnectionString("ECome"));
            });

            // 2. Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 3. ⬇️⬇️⬇️ ضيف السطور دي بالظبط ⬇️⬇️⬇️
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
      

            // 4. UnitOfWork
            services.AddScoped<IUintOfWork, UnitOfWork>();

            return services;
        }
    }
}