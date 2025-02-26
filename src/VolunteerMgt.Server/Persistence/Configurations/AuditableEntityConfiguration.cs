using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Persistence.Configurations;

public class AuditableEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity> where TEntity : AuditableEntity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedOnAdd()
               .IsRequired();

        builder.Property(c => c.CreatedDate).IsRequired();
        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(256);

        builder.Property(c => c.ModifiedDate).IsRequired(false);
        builder.Property(c => c.ModifiedBy).IsRequired(false).HasMaxLength(256);
    }
}