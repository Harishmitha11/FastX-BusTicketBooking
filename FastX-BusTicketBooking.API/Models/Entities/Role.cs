using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Role name should contain only letters and spaces.")]
        public string RoleName { get; set; } = null!;
    }
}
