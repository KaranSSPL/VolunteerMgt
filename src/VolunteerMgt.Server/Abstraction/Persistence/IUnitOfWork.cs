﻿using VolunteerMgt.Server.Abstraction.Service.Common;

namespace VolunteerMgt.Server.Abstraction.Persistence;

public interface IUnitOfWork : IScopedService
{
    //IBaseRepository<EntityName, EntityKeyType> EntityNames { get; }
}
