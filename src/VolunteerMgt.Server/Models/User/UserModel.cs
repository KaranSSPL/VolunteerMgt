using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Models.User
{
    public class UserModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Roles { get; set; }
        [Required]
        public string Email { get; set; }
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits and only contain numeric characters.")]
        [Required]
        public string Phone { get; set; }
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}", ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
