using E_com_Core.Model.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Data.Configurations
{
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(30);
            builder.Property(c => c.Id).IsRequired();
            builder.HasData(
                new Category { Id = 1, Name = "test", Description = "tests" },
                new Category { Id = 2, Name = "phone", Description = "phones" });
        }
    }
}
