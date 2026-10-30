using E_com_Core.Model.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Data.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(30);
            builder.Property(p => p.Id).IsRequired();     
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.NewPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.OldPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.HasData(
               new Product { Id = 1, Name = "tv", Description = "tv dell" , NewPrice= 20000 , CategoryId=1 },
               new Product { Id = 2, Name = "ios", Description = "ios mobile" , NewPrice = 30000 , CategoryId = 1 },
               new Product { Id = 9, Name = "tvs", Description = "ios sss" , NewPrice = 60000 , CategoryId = 7 }
               );
            builder.Property((p) => p.Barcode).HasMaxLength(32);
            builder.HasIndex((p) => p.Barcode).IsUnique();
           
        }
    }
}
