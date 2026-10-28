using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Shering
{
    public class EmailStringBady
    {
        public static string Send(string email, string token, string component, string message)
        {
            string encodedToken = Uri.EscapeDataString(token); // التشفير هنا كافي جداً

            return $@"
<html>
  <body style='font-family:Arial,sans-serif;'>
    <h1>{message}</h1>
    <a href='http://localhost:4200/Account/{component}?email={email}&token={encodedToken}'
       style='display:inline-block;padding:10px 20px;background:#4f46e5;color:#fff;border-radius:8px;text-decoration:none;'>
      {message}
    </a>
  </body>
</html>";
        }
    }
}
