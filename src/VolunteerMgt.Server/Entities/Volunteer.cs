using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Entities
{
    public class Volunteer
    {
        [Required]
        [SwaggerSchema("Email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [SwaggerSchema("Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.PhoneNumber)]
        [SwaggerSchema("Phonenumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [SwaggerSchema("Role")]
        public string Role { get; set; } = string.Empty;
    }
}
