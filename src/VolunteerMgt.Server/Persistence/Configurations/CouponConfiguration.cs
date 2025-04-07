using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupons>
    {
        public void Configure(EntityTypeBuilder<Coupons> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Date)
                   .IsRequired();

            builder.Property(c => c.CouponValue)
                   .IsRequired();

            builder.HasMany(c => c.AdditionalCoupons)
                   .WithOne(ac => ac.Coupon)
                   .HasForeignKey(ac => ac.CouponId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
