using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasSizes { get; set; }
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();

    }
}
