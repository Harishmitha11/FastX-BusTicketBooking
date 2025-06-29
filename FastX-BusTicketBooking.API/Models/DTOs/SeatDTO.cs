using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class SeatDTO
    {
        public int SeatId { get; set; } // Optional for creation

        [Required(ErrorMessage = "Route ID is required.")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Seat number is required.")]
        [StringLength(10, ErrorMessage = "Seat number must not exceed 10 characters.")]
        public string SeatNumber { get; set; } = null!;

        public bool IsBooked { get; set; } = false;
    }
}
