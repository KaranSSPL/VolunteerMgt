using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Name)
                   .IsRequired();

            builder.Property(v => v.MobileNo)
                   .IsRequired();

            builder.Property(v => v.Address)
                   .HasMaxLength(255);

            builder.Property(v => v.Occupation)
                   .HasMaxLength(100);

            builder.Property(v => v.ImagePath)
                   .HasMaxLength(255);

            builder.Property(v => v.code)
                   .HasMaxLength(50);

            builder.Property(v => v.VolunteerType)
                   .HasMaxLength(50);

            builder.Ignore(v => v.Image);

            builder.HasMany(v => v.Availabilities)
                   .WithOne(a => a.Volunteer)
                   .HasForeignKey(a => a.VolunteerId);
        }
    }
}
