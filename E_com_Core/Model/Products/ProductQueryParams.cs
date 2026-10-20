using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class ProductQueryParams
    {
        public string Sort { get; set; } = "default";
        public string? Search { get; set; }
        public int CategoryId { get; set; } = 0;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
