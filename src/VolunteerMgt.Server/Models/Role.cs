using VolunteerMgt.Server.Entities.Identity;

namespace VolunteerMgt.Server.Models
{
    public class Role
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
