using E_com_Core.Model.User;
using E_com_Core.Shering;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository.Services
{
    public class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public GenerateToken(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        // ✅ بقت async Task<string>
        public async Task<string> GetAndCreateToken(AppUser user)
        {
            // 1. الـ Claims الأساسية
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // ✅ 2. هات أدوار اليوزر وضيفها في التوكن
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. الـ Secret Key
            var security = _configuration["Token:Secret"];
            var key = Encoding.ASCII.GetBytes(security);

            SigningCredentials credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );

            // 4. وصف التوكن
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                Issuer = _configuration["Token:Issure"],
                SigningCredentials = credentials,
                NotBefore = DateTime.Now,
            };

            // 5. إنشاء التوكن
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}