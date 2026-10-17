using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Interfases
{
    public interface IUintOfWork
    {
        public ICategoryRepository categoryRepository { get; }
        public IProductRepository  productRepository { get; }
        public IPhotoRepository  photoRepository { get; }
      
    }
}
