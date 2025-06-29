using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Bus
    {
        [Key]
        public int BusId { get; set; } // IDENTITY(101,1)

        [Required(ErrorMessage = "Bus name is required.")]
        [StringLength(100, ErrorMessage = "Bus name must not exceed 100 characters.")]
        public string BusName { get; set; } = null!;

        [Required(ErrorMessage = "Bus number is required.")]
        [StringLength(50, ErrorMessage = "Bus number must not exceed 50 characters.")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z]{1,2}\d{4}$", ErrorMessage = "Bus number must follow format like TN01AB1234.")]
        public string BusNumber { get; set; } = null!;

        [Required(ErrorMessage = "Bus type is required.")]
        [StringLength(50, ErrorMessage = "Bus type must not exceed 50 characters.")]
        public string BusType { get; set; } = null!;

        [Required(ErrorMessage = "Total seats are required.")]
        [Range(1, 1000, ErrorMessage = "Total seats must be between 1 and 1000.")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Amenities description is required.")]
        [StringLength(255, ErrorMessage = "Amenities must not exceed 255 characters.")]
        public string Amenities { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        // Navigation property
        public ICollection<Route>? Routes { get; set; }
    }
}
