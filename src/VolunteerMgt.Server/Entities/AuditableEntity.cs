using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities;

public class AuditableEntity<TKey> : IAuditEntity, IBaseEntity<TKey>
{
    public TKey Id { get; set; } = default!;

    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = default!;

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
