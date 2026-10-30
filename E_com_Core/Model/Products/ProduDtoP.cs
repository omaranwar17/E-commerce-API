using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class ProduDtoP
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Barcode { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public string CategoryName { get; set; } 
        public List<PhotoDtos> Photos { get; set; }
    }
}
