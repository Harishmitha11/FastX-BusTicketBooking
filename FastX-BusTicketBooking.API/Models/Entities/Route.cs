using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastX_BusTicketBooking.API.Models.Entities
{
    public class Route
    {
        [Key]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Bus ID is required.")]
        public int BusId { get; set; }

        [ForeignKey("BusId")]
        public Bus Bus { get; set; } = null!;

        [Required(ErrorMessage = "Origin is required.")]
        [StringLength(100, ErrorMessage = "Origin cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Origin should contain only letters and spaces.")]
        public string Origin { get; set; } = null!;

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Destination should contain only letters and spaces.")]
        public string Destination { get; set; } = null!;

        [Required(ErrorMessage = "Departure time is required.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required.")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Fare is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Fare must be greater than zero.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Fare { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
