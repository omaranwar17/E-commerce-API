
using E_com_Core.Interfases;
using E_com_Core.Services;
using E_com_Infrastructurer;
using E_com_Infrastructurer.Repository;
using E_come_API.Mapping;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace E_come_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors((op) =>
            {
                op.AddPolicy("CORSPolicy", (b) =>
                {
                    b.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200");
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
            builder.Services.InfrastructurerColl(builder.Configuration);
           
            builder.Services.AddScoped<IImageServices, ImageServices>();
            builder.Services.AddScoped<ICustomerBasketRepository, CustomerBasketRepository>();

            builder.Services.AddSingleton<IFileProvider>(
              new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
          );

            builder.Services.AddMemoryCache();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
             }
            app.UseCors("CORSPolicy");
            app.UseStaticFiles();

            app.UseMiddleware<E_com_API.Middleware.ExceptionsMiddleware>();
                app.UseStatusCodePagesWithReExecute("/errors/{0}");

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
        }
    }

