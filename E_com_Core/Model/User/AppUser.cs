using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.User
{
    public class AppUser:IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public Address? Address { get; set; }
    }
}
