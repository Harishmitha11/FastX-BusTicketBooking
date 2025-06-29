using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class RefundDTO
    {
        public int RefundId { get; set; }  // Only needed for response/view

        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Refund amount is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Refund amount must be greater than zero.")]
        public decimal RefundAmount { get; set; }

        [Required(ErrorMessage = "Refund date is required.")]
        public DateTime RefundDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "ProcessedBy User ID is required.")]
        public int ProcessedBy { get; set; }
    }
}
