using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities;

public class BaseEntity : BaseEntity<int> 
{

}

public class BaseEntity<TKey> : IBaseEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}