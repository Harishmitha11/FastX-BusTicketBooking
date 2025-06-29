using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class RouteDTO
    {
        public int RouteId { get; set; } // Optional for create

        [Required(ErrorMessage = "BusId is required.")]
        public int BusId { get; set; }

        [Required(ErrorMessage = "Origin is required.")]
        [StringLength(100, ErrorMessage = "Origin cannot exceed 100 characters.")]
        public string Origin { get; set; } = null!;

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; } = null!;

        [Required(ErrorMessage = "Departure time is required.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required.")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Fare is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Fare must be greater than 0.")]
        public decimal Fare { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
