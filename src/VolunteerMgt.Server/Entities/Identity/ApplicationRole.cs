using Microsoft.AspNetCore.Identity;
using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities.Identity;

public class ApplicationRole : IdentityRole, IAuditEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = default!;

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public ApplicationRole()
    {

    }
    public ApplicationRole(string roleName, string username) : base(roleName)
    {
        CreatedDate = DateTime.UtcNow;
        CreatedBy = username;
    }
}
