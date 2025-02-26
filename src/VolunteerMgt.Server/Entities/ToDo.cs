﻿using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Entities;

public class ToDo : IBaseEntity<long>, IAuditableEntity
{
    public long Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
