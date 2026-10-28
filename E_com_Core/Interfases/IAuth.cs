using E_com_Core.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Interfases
{
    public interface IAuth
    {
        Task<string> RegisterAsync(RegisterDT registerDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string> ConfirmEmailAsync(string email, string token);
       Task<bool> SendEmailForgetPassword(string email);
       Task<bool> ActiveAccount(ActiveAccountDTO accountDTO);
      Task<string> ResetPassword(ResetPasswordDTO resetPassword);
    }
}
