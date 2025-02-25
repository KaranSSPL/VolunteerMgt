namespace VolunteerMgt.Server.Entities;

public class ToDo : BaseEntity<long>
{
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
}
