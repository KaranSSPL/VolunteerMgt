namespace VolunteerMgt.Server.Abstraction.Entity;

public interface IBaseEntity<TKey>
{
    TKey Id { get; set; }
}