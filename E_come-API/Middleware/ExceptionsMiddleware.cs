using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace E_com_API.Middleware
{
    public class ExceptionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);

        public ExceptionsMiddleware(RequestDelegate next, IHostEnvironment environment, IMemoryCache memoryCache)
        {
            _next = next;
            _environment = environment;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (!IsRequestAllowed(context))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsJsonAsync(new { message = "Too many requests. Please try again later." });
                    return;
                }

                ApplySecurity(context);

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var message = _environment.IsDevelopment() ? ex.Message : "An internal server error has occurred.";

                await context.Response.WriteAsJsonAsync(new { message = message });
            }
        }

        private bool IsRequestAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"Rate:{ip}";
            var dateNow = DateTime.Now;

            var (tokenCount, timeStamp) = _memoryCache.TryGetValue(cacheKey, out Tuple<int, DateTime>? cacheEntry)
              ? cacheEntry!
              : Tuple.Create(0, dateNow);

            if (dateNow - timeStamp > _rateLimitWindow)
            {
                tokenCount = 0;
                timeStamp = dateNow;
            }

            if (tokenCount >= 8) // 8 طلبات كحد أقصى في الـ 30 ثانية
            {
                return false;
            }

            tokenCount++;
            _memoryCache.Set(cacheKey, Tuple.Create(tokenCount, timeStamp), _rateLimitWindow);
            return true;
        }

        private void ApplySecurity(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }
    }
}