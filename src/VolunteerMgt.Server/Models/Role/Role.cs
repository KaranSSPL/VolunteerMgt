﻿namespace VolunteerMgt.Server.Models.Role
{
    public class Role
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    public class AssignUser
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = new();
    }
}
