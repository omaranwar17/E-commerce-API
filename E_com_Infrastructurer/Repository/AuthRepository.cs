using E_com_Core.Interfases;
using E_com_Core.Model.User;
using E_com_Core.Services;
using E_com_Core.Shering;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace E_com_Infrastructurer.Repository
{
    public class AuthRepository : IAuth
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signIn;
        private readonly IGenerateToken  _token1;

        public AuthRepository(UserManager<AppUser> userManager,IGenerateToken token , IEmailService emailService, SignInManager<AppUser> signIn)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signIn = signIn;
            _token1 = token;
        }

        public async Task<string> RegisterAsync(RegisterDT registerDTO)
        {
            if (registerDTO == null)
                throw new ArgumentNullException(nameof(registerDTO));

            if (await _userManager.FindByNameAsync(registerDTO.UserName) is not null)
                return "Username is already taken";

            if (await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
                return "Email is already registered";

            var user = new AppUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName,
                DisplayName = registerDTO.DisplayName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            // Send Active Email
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmail(
                registerDTO.Email,
                code,
                "active",
                "Active Email",
                "Please active your email, click on button to active");

            return "Registered successfully";
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            ArgumentNullException.ThrowIfNull(loginDTO);

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user is null)
                return "Invalid email or password";

            // lockoutOnFailure: true => بعد 5 محاولات غلط الحساب بيتقفل 5 دقايق (الافتراضي)
            var result = await _signIn.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return "Account is locked. Please try again later";

            if (!result.Succeeded)
                return "Invalid email or password";

            //بنتحقق من التأكيد بعد الباسورد عشان ما نكشفش إن الإيميل مسجل لشخص غريب
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await SendEmail(
                    user.Email!,
                    token,
                    "active",
                    "Active Email",
                    "Please active your email, click on button to active");

                return "Please confirm your email first, we have sent an activation link to your E-mail";
            }

            return await _token1.GetAndCreateToken(user);
        }

        public async Task<string> ConfirmEmailAsync(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return "Invalid request";

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return "Invalid request";

            if (await _userManager.IsEmailConfirmedAsync(user))
                return "Email is already confirmed";

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded
                ? "Email confirmed successfully"
                : "Invalid or expired token";
        }

        private async Task SendEmail(string email, string code, string component, string subject, string message)
        {
            // شيلنا Uri.EscapeDataString من هنا
            var result = new EmailDto(
                to: email,
                from: "alkermomar54@gmail.com", // تأكد إن ده مطابق لإعدادات SMTP
                subject: subject,
                content: EmailStringBady.Send(email, code, component, message)); // الدالة دي هتشفر لوحدها

            await _emailService.SendEmail(result);
        }

        public async Task<bool> SendEmailForgetPassword(string email)
        {
            var findUser = await _userManager.FindByEmailAsync(email);
            if (findUser is null)
            {
                Console.WriteLine($"User not found: {email}");
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(findUser);

            try
            {
                await SendEmail(findUser.Email!, token, "Reset-Password", "Reset-Password", "click ResetPassword");
                Console.WriteLine("Email sent OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex}");
                throw;
            }

            return true;
        }

       

        public async Task<string> ResetPassword(ResetPasswordDTO resetPassword)
        {
            // 1. تصحيح الغلطة المطبعية هنا (كان مكتوب var appUser findUser)
            var findUser = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (findUser is null)
            {
                return "Invalid or expired token";
            }

            // 2. إعادة تعيين الباسورد
            var result = await _userManager.ResetPasswordAsync(findUser, resetPassword.Token, resetPassword.Password);

            // 3. لو نجحت العملية
            if (result.Succeeded)
            {
                return "Password change success";
            }

            // 4. النقطة اللي الراجل بيتكلم عنها (عرض سبب الخطأ)
            // لو فشلت، بيرجع أول سبب للخطأ (زي: الباسورد ضعيف، أو التوكن غير صالح)
            return result.Errors.ToList()[0].Description;
        }

        public async Task<bool> ActiveAccount(ActiveAccountDTO accountDTO)
        {
            // 1. تصحيح الغلطة المطبعية
            var findUser = await _userManager.FindByEmailAsync(accountDTO.Email);

            if (findUser is null)
            {
                return false;
            }

            // 2. تصحيح نوع المتغير
            var result = await _userManager.ConfirmEmailAsync(findUser, accountDTO.Token);

            if (result.Succeeded)
            {
                return true;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(findUser);
            await SendEmail(
                   findUser.Email!,
                   token,
                   "active",
                   "Active Email",
                   "Please active your email, click on button to active");


            return false;
        }

    }
}