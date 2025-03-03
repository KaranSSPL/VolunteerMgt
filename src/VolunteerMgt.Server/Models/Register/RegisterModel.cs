using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Models.Register
{
    public class RegisterModel : VolunteerModel
    {
        [Required]
        [SwaggerSchema("Password")]
        public string Password { get; set; } = string.Empty;

    }
}
