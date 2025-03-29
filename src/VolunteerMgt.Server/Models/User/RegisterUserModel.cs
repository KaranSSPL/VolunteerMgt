using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Models.Volunteer
{
    public class RegisterUserModel : Entities.User
    {
        [Required]
        [SwaggerSchema("Password")]
        public string Password { get; set; } = string.Empty;

    }
}
