using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10, ErrorMessage = "Gender must not exceed 10 characters.")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Contact number must start with 6-9 and be 10 digits.")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address must not exceed 255 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;
    }
}
