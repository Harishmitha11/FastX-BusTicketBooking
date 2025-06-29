using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class BookingSeat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required(ErrorMessage = "Seat ID is required.")]
        public int SeatId { get; set; }

        [ForeignKey("SeatId")]
        public Seat Seat { get; set; } = null!;

        public bool IsCancelled { get; set; } = false;
    }
}
