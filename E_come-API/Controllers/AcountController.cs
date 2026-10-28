using AutoMapper;
using E_com_Core.Interfases;
using E_com_Core.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_come_API.Controllers
{
    public class AcountController : BaseController
    {
        public AcountController(IUintOfWork work, IMapper mapper) : base(work, mapper)
        { }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDT dto)
        {
            var result = await work.auth.RegisterAsync(dto);
            return result == "Registered successfully"
                ? Ok(new { message = result })
                : BadRequest(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await work.auth.LoginAsync(dto);

            // التوكن (JWT) فيه نقطتين بالظبط، الرسائل العادية لأ
            if (result.Count(c => c == '.') != 2)
                return Unauthorized(new { message = result });

            Response.Cookies.Append("token", result, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,   // 👈 بدل Strict
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Ok(new { message = "Logged in successfully" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok(new { message = "Logged out" });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await work.auth.ConfirmEmailAsync(email, token);
            return result.StartsWith("Email confirmed") || result == "Email is already confirmed"
                ? Ok(new { message = result })
                : BadRequest(new { message = result });
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccount(ActiveAccountDTO dto)
            => await work.auth.ActiveAccount(dto)
                ? Ok(new { message = "Account activated" })
                : BadRequest(new { message = "Invalid or expired token. A new link was sent if the account exists." });

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromQuery] string email)
        {
            await work.auth.SendEmailForgetPassword(email);
            // always the same response
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await work.auth.ResetPassword(dto);
            return result == "Password change success"
                ? Ok(new { message = result })
                : BadRequest(new { message = result });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                userName = User.Identity?.Name,
                roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value)
            });
        }
    }
}