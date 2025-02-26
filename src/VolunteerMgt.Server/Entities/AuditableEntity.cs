using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities;

public class AuditableEntity : IAuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
