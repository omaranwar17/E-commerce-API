using E_com_Core.Model.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Data.Context
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) :base(options)
        {
            
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product>  Products { get; set; }
        public virtual DbSet<Photo>  Photos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());        }
       
    }
}
