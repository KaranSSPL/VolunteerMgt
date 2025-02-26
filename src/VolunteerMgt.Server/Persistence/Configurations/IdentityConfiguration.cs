using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities.Identity;

namespace VolunteerMgt.Server.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(p => p.PhoneNumber).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.SecurityStamp).HasMaxLength(512).IsRequired();
        builder.Property(p => p.ConcurrencyStamp).HasMaxLength(512).IsConcurrencyToken().IsRequired();
                
        builder.Property(c => c.CreatedDate).IsRequired();
        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(256);

        builder.Property(c => c.ModifiedDate).IsRequired(false);
        builder.Property(c => c.ModifiedBy).IsRequired(false).HasMaxLength(256);
    }
}

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles");

        builder.Property(p => p.ConcurrencyStamp).HasMaxLength(1024);

        builder.Property(c => c.CreatedDate).IsRequired();
        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(256);

        builder.Property(c => c.ModifiedDate).IsRequired(false);
        builder.Property(c => c.ModifiedBy).IsRequired(false).HasMaxLength(256);
    }
}

public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        builder.ToTable("RoleClaims");

        builder.Property(p => p.ClaimType).HasMaxLength(1024).IsRequired(false);
        builder.Property(p => p.ClaimValue).HasMaxLength(1024).IsRequired(false);
    }
}

public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder) =>
        builder.ToTable("UserRoles");
}

public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.ToTable("UserClaims");

        builder.Property(p => p.ClaimType).HasMaxLength(1024).IsRequired(false);
        builder.Property(p => p.ClaimValue).HasMaxLength(1024).IsRequired(false);
    }
}

public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        builder.ToTable("UserLogins");
        builder.Property(p => p.ProviderDisplayName).HasMaxLength(256).IsRequired(false);
    }
}

public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder) =>
        builder.ToTable("UserTokens");
}