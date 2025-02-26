using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Abstraction.Persistence;

public interface IUnitOfWork: IScopedService
{
    IBaseRepository<ToDo, long> ToDos { get; }
}
