using E_com_Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Interfases
{
    public interface IInvoiceService
    {
        byte[] Generate(OrderDto order);
    }
}
