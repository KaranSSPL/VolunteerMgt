namespace VolunteerMgt.Server.Abstraction.Entity;

public interface IAuditEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
