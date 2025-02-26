using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities;

public class BaseEntity: IBaseEntity<int>
{
    public int Id { get; set; }
}