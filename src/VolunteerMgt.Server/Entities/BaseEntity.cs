namespace VolunteerMgt.Server.Entities;

public class BaseEntity<Tkey>
{
    public Tkey Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
