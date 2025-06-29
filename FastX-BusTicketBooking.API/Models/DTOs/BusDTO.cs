using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class BusDTO
    {
        [Key]
        [Required(ErrorMessage = "BusId is required.")]
        public int BusId { get; set; }

        [Required(ErrorMessage = "Bus name is required.")]
        [StringLength(100, ErrorMessage = "Bus name must not exceed 100 characters.")]
        public string BusName { get; set; } = null!;

        [Required(ErrorMessage = "Bus number is required.")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z]{1,2}\d{4}$",
            ErrorMessage = "Enter a valid bus number (e.g., TN01AB1234).")]
        [StringLength(50)]
        public string BusNumber { get; set; } = null!;

        [Required(ErrorMessage = "Bus type is required.")]
        [StringLength(50, ErrorMessage = "Bus type must not exceed 50 characters.")]
        public string BusType { get; set; } = null!;

        [Required(ErrorMessage = "Total seats are required.")]
        [Range(1, 1000, ErrorMessage = "Total seats must be at least 1.")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Amenities are required.")]
        [StringLength(255, ErrorMessage = "Amenities must not exceed 255 characters.")]
        public string Amenities { get; set; } = null!;

        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
    }
}
