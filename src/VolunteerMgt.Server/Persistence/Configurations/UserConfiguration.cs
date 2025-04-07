using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Firstname)
                   .IsRequired();

            builder.Property(u => u.Lastname)
                   .IsRequired();

            builder.Property(u => u.Username)
                   .IsRequired();

            builder.Property(u => u.Roles)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .IsRequired();

            builder.Property(u => u.Phone)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(u => u.Password)
                   .HasMaxLength(255);
        }
    }
}
