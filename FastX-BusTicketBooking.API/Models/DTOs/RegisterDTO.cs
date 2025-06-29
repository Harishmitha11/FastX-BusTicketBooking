using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Contact number must start with 6-9 and be 10 digits.")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Role ID is required.")]
        public int RoleId { get; set; }
    }
}
