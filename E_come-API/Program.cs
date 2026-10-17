
using E_com_Core.Services;
using E_com_Infrastructurer;
using E_com_Infrastructurer.Repository;
using E_come_API.Mapping;
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
            builder.Services.InfrastructurerColl(builder.Configuration);
            builder.Services.AddScoped<IImageServices, ImageServices>();

            builder.Services.AddSingleton<IFileProvider>(
              new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
          );


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
