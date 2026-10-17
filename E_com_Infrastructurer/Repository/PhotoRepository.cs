using E_com_Core.Interfases;
using E_com_Core.Model.Products;
using E_com_Infrastructurer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(StoreDbContext storeDb) : base(storeDb)
        {
        }
    }
}
