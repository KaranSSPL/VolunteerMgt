using Microsoft.AspNetCore.Identity;
using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities.Identity;

public class ApplicationUser : IdentityUser, IAuditEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = default!;

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
