using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.User;
using VolunteerMgt.Server.Models.Volunteers;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Persistence;

public class DatabaseContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DatabaseContext() { }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<UserModel> User { get; set; }
    
    public DbSet<AvailabilityModel> Availability { get; set; }

    public DbSet<VolunteerModel> Volunteer { get; set; }

    public DbSet<ServiceModel> Service { get; set; }

    public DbSet<VolunteerServiceMapping> VolunteerServiceMapping { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}