using E_com_Core.Interfases;
using E_com_Core.Services;
using E_com_Core.Shering;
using E_com_Infrastructurer.Data.Context;
using E_com_Infrastructurer.Repository;
using E_com_Infrastructurer.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

            services.AddSingleton<IConnectionMultiplexer>((IServiceProvider i) =>
            {
                // سحب الـ connection string باستخدام الـ i (اللي هو الـ IServiceProvider)
                var connectionString = i.GetRequiredService<IConfiguration>().GetConnectionString("redis");

                // تحويل النص لإعدادات
                var config = ConfigurationOptions.Parse(connectionString);

                // عمل الاتصال
                return ConnectionMultiplexer.Connect(config);
            });

            // 3. ⬇️⬇️⬇️ ضيف السطور دي بالظبط ⬇️⬇️⬇️
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IGenerateToken, GenerateToken>();
      

            // 4. UnitOfWork
            services.AddScoped<IUintOfWork, UnitOfWork>();

           

            return services;
        }
    }
}