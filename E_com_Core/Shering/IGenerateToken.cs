using E_com_Core.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Shering
{
    public interface IGenerateToken
    {
        Task<string> GetAndCreateToken(AppUser user);
    }
}
