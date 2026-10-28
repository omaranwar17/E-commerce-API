using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class Size : BaseEntity<int>
    {
        public string Name { get; set; }   // S, M, L, XL ...
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
