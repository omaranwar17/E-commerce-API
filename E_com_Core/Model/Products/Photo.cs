using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class Photo: BaseEntity<int>
    {
        public string ImageName { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Products { get; set; }
    }
}
