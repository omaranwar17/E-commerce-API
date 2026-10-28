using E_com_Core.Interfases;
using E_com_Core.Model.User;
using E_com_Core.Services;
using E_com_Infrastructurer;
using E_com_Infrastructurer.Data.Context;
using E_com_Infrastructurer.Repository;
using E_com_Infrastructurer.Repository.Services;
using E_come_API.Mapping;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            })
            .AddEntityFrameworkStores<StoreDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.InfrastructurerColl(builder.Configuration);

            builder.Services.AddScoped<IImageServices, ImageServices>();
            builder.Services.AddScoped<IAuth, AuthRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICustomerBasketRepository, CustomerBasketRepository>();

            builder.Services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );

            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = false;
                op.SaveToken = true;

                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Secret"])),

                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Token:Issure"],

                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                };

                // قراءة التوكن من الكوكيز
                op.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });

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
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseStatusCodePages();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}