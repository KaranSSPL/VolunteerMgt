using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Models.User
{
    public class EditUserModel : Entities.User
    {
        [Required]
        [SwaggerSchema("User Id")]
        public string Id { get; set; } = string.Empty;

    }
}
