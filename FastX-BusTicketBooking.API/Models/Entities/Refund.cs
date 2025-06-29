using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Refund
    {
        [Key]
        public int RefundId { get; set; }

        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required(ErrorMessage = "Refund amount is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Refund amount must be greater than 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal RefundAmount { get; set; }

        [Required(ErrorMessage = "Refund date is required.")]
        public DateTime RefundDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "ProcessedBy (user ID) is required.")]
        public int ProcessedBy { get; set; }

        [ForeignKey("ProcessedBy")]
        public User ProcessedByUser { get; set; } = null!;
    }
}
