using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Roles { get; set; }
        public string Email { get; set; }

        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits and only contain numeric characters.")]
        public string Phone { get; set; }

        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}", ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
