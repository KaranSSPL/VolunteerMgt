using VolunteerMgt.Server.Entities.Identity;

namespace VolunteerMgt.Server.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
