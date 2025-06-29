using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class CancelSeatsDTO
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "At least one seat ID must be provided.")]
        [MinLength(1, ErrorMessage = "You must select at least one seat to cancel.")]
        public List<int> SeatIds { get; set; } = new List<int>();
    }
}
