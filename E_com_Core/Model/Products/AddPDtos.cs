using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class AddPDtos
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public string? Barcode { get; set; }
        public int CategoryId { get; set; }
        //public ProductSize? Size { get; set; }
        public List<ProductSize>? Sizes { get; set; }
        public IFormFileCollection Photos { get; set; }
    }
}
