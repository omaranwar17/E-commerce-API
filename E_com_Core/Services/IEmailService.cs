using E_com_Core.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Services
{
    public interface IEmailService
    {
        Task SendEmail(EmailDto emailDTO);
    }
}
