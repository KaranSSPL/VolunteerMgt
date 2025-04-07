using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations
{
    public class AdditionalCouponConfiguration : IEntityTypeConfiguration<AdditionalCoupon>
    {
        public void Configure(EntityTypeBuilder<AdditionalCoupon> builder)
        {
            builder.HasKey(ac => ac.Id);

            builder.Property(ac => ac.AdditionalCouponValue)
                   .IsRequired();

            builder.Property(ac => ac.CreatedDate)
                   .IsRequired();

            builder.Ignore(ac => ac.TotalValue); 

            builder.HasOne(ac => ac.Coupon)
                   .WithMany(c => c.AdditionalCoupons)
                   .HasForeignKey(ac => ac.CouponId);
        }
    }
}
