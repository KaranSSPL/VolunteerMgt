using Microsoft.AspNetCore.Identity;
using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities.Identity;

public class ApplicationRole : IdentityRole, IAuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
