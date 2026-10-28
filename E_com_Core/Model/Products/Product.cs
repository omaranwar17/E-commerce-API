using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Products
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        //public ProductSize? Size { get; set; }   // nullable، بيتملي بس لو الكاتجري هدوم
        public string? Sizes { get; set; } // هيخزن زي "S,M,L" 
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Categorys { get; set; }
        public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
    }
}
