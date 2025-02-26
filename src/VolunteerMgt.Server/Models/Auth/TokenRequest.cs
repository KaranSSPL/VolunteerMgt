using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Models.Auth;

public class TokenRequest
{
    [Required]
    [SwaggerSchema("Email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema("Password")]
    public string Password { get; set; } = string.Empty;
}
