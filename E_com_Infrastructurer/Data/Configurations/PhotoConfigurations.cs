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
    internal class PhotoConfigurations : IEntityTypeConfiguration<Photo>
    {
       

        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.Property(c => c.ImageName).IsRequired();
            builder.Property(c => c.Id).IsRequired();
            builder.HasData(
                new Photo { Id = 1, ImageName = "test" , ProductId = 8 },
                new Photo { Id = 2, ImageName = "phone" , ProductId = 8});
        }
    }
}
