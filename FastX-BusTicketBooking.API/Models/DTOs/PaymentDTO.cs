using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class PaymentDTO
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [StringLength(50, ErrorMessage = "Payment method must be up to 50 characters.")]
        public string PaymentMethod { get; set; } = null!;
    }
}
