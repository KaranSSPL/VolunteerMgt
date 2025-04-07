using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.VolunteerService;
using VolunteerMgt.Server.Persistence.Configurations;

namespace VolunteerMgt.Server.Persistence;

public class DatabaseContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DatabaseContext() { }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<User> User { get; set; }
    
    public DbSet<Availability> Availability { get; set; }

    public DbSet<Volunteer> Volunteer { get; set; }

    public DbSet<Service> Service { get; set; }

    public DbSet<VolunteerServiceMapping> VolunteerServiceMapping { get; set; }

    public DbSet<Coupons> Coupons { get; set; }

    public DbSet<AdditionalCoupon> AdditionalCoupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CouponConfiguration());
        modelBuilder.ApplyConfiguration(new AdditionalCouponConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VolunteerConfiguration());
        modelBuilder.ApplyConfiguration(new AvailabilityConfiguration());
        modelBuilder.ApplyConfiguration(new ServiceConfiguration());
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}