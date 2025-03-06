using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Models.PasswordModel
{
    public class ForgotPasswordModel
    {
        [Required]
        [SwaggerSchema("Email")]
        public string Email { get; set; } = string.Empty;
    }
}
