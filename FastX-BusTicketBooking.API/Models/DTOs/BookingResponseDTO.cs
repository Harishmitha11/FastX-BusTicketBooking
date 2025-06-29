using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class BookingResponseDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be a positive number.")]
        public int BookingId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a valid positive number.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Route ID must be a valid positive number.")]
        public int RouteId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "At least one seat must be booked.")]
        public int NoOfSeats { get; set; }

        [Required]
        [Range(1.00, 999999.99, ErrorMessage = "Total fare must be a valid amount greater than ₹1.")]
        public decimal TotalFare { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Status can be at most 20 characters.")]
        [RegularExpression("^(Booked|Cancelled)$", ErrorMessage = "Status must be 'Booked' or 'Cancelled'.")]
        public string Status { get; set; } = "Booked";
    }
}
