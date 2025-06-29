using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email must be a valid format and up to 100 characters.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must have at least one uppercase letter, one number, and one special character.")]

     
        public string Password { get; set; } = null!;

        [JsonIgnore]
        public string? Token { get; set; }

        [JsonIgnore]
        public int? UserId { get; set; }

        [JsonIgnore]
        public string? Role { get; set; }
    }
}
