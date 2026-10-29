using E_com_Core.Model.Checkout;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.Subtotal).HasPrecision(18, 2);
        builder.Property(x => x.ShippingFee).HasPrecision(18, 2);
        builder.Ignore(x => x.Total);

        builder.Property(x => x.BuyerEmail).IsRequired().HasMaxLength(256);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey("OrderId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(x => x.ShippingAddress, a =>
        {
            a.Property(p => p.FullName).IsRequired().HasMaxLength(100);
            a.Property(p => p.Phone).IsRequired().HasMaxLength(15);
            a.Property(p => p.Governorate).IsRequired().HasMaxLength(50);
            a.Property(p => p.City).IsRequired().HasMaxLength(100);
            a.Property(p => p.Street).IsRequired().HasMaxLength(250);
            a.Property(p => p.Notes).HasMaxLength(500);
        });

        builder.Property(x => x.PaymentMethod).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30);
        builder.Property(x => x.PaymentReference).HasMaxLength(100);

        // رقم العملية لا يتكرر. الفلتر بيسمح بعدد غير محدود من الأوردرات
        // اللي PaymentReference فيها null (الدفع عند الاستلام).
        builder.HasIndex(x => x.PaymentReference)
               .IsUnique()
               .HasFilter("[PaymentReference] IS NOT NULL");
    }
}