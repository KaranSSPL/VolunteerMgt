using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations
{
    public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
    {
        public void Configure(EntityTypeBuilder<Availability> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Day)
                   .HasMaxLength(20);

            builder.Property(a => a.TimeSlot)
                   .HasMaxLength(50);

            builder.HasOne(a => a.Volunteer)
                   .WithMany(v => v.Availabilities)
                   .HasForeignKey(a => a.VolunteerId);
        }
    }
}
