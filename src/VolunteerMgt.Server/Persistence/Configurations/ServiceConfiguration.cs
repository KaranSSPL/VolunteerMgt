using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ServiceName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.SaturdayVolunteerRequirement)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.SundayVolunteerRequirement)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.EkadashiVolunteerRequirement)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.FestivalVolunteerRequirement)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.DefaultTime)
                   .IsRequired();

            builder.Property(s => s.Code)
                   .IsRequired();

            builder.HasMany(s => s.VolunteerMappings)
                   .WithOne(vm => vm.Service)
                   .HasForeignKey(vm => vm.ServiceId);
        }
    }
}
