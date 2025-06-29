using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        [Required(ErrorMessage = "Route ID is required.")]
        public int RouteId { get; set; }

        [ForeignKey("RouteId")]
        public Route Route { get; set; } = null!;

        [Required(ErrorMessage = "Seat number is required.")]
        [StringLength(10, ErrorMessage = "Seat number cannot exceed 10 characters.")]
        [RegularExpression(@"^S\d+$", ErrorMessage = "Seat number must start with 'S' followed by digits (e.g., S1, S25).")]
        public string SeatNumber { get; set; } = null!;

        public bool IsBooked { get; set; } = false;
    }
}
