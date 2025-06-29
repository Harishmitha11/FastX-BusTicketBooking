using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [StringLength(20, ErrorMessage = "Payment method must not exceed 20 characters.")]
        [RegularExpression(@"^(Cash|Card|UPI|NetBanking)$", ErrorMessage = "Allowed methods: Cash, Card, UPI, NetBanking.")]
        public string PaymentMethod { get; set; } = "UPI";

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20, ErrorMessage = "Status must not exceed 20 characters.")]
        [RegularExpression(@"^(Completed|Pending|Failed)$", ErrorMessage = "Status must be either Completed, Pending, or Failed.")]
        public string Status { get; set; } = "Completed";
    }
}
