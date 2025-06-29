using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required(ErrorMessage = "Route ID is required.")]
        public int RouteId { get; set; }

        [ForeignKey("RouteId")]
        public Route Route { get; set; } = null!;

        [Required(ErrorMessage = "Booking date is required.")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Number of seats is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please book at least one seat.")]
        public int NoOfSeats { get; set; }

        [Required(ErrorMessage = "Total fare is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Total fare must be greater than 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalFare { get; set; }

        [Required(ErrorMessage = "Booking status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        [RegularExpression(@"^(Booked|Cancelled)$", ErrorMessage = "Status must be either 'Booked' or 'Cancelled'.")]
        public string Status { get; set; } = "Booked";
    }
}
