using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters, include uppercase, lowercase, number, and special character.")]
        public string PasswordHash { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Contact number is required.")]
        [StringLength(20)]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 6-9.")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address must be under 255 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "RoleId is required.")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
    }
}
