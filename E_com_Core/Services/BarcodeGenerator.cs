using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Services
{
    public static class BarcodeGenerator
    {
        // 200 (internal-use prefix) + 9-digit zero-padded id + check digit = EAN-13
        public static string GenerateEan13(int productId)
        {
            var body = $"200{productId:D9}";
            var sum = 0;
            for (var i = 0; i < 12; i++)
            {
                var digit = body[i] - '0';
                sum += i % 2 == 0 ? digit : digit * 3;
            }
            return body + (10 - sum % 10) % 10;
        }
    }
}
