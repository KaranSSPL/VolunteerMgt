using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Entities.Identity;

namespace VolunteerMgt.Server.Persistence;

public class DatabaseContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DatabaseContext() { }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<PermissionRoles> PermissionRoles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}