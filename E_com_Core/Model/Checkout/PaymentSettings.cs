using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public class PaymentSettings
    {
        public string VodafoneCashNumber { get; set; } = string.Empty;
        public string InstaPayAddress { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
    }
}
