using System.ComponentModel.DataAnnotations;

namespace FastX_BusTicketBooking.API.Models.DTOs
{
    public class BookingRequestDTO
    {
        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid User ID.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Route ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Route ID.")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Please enter the number of seats to book.")]
        [Range(1, 1000, ErrorMessage = "You must book at least one seat (up to 1000 allowed).")]
        public int NoOfSeats { get; set; }

        //[Required(ErrorMessage = "Booking status is required.")]
        //[StringLength(20, ErrorMessage = "Status must be 1 to 20 characters long.")]
        //[RegularExpression("^(Booked|Cancelled)$", ErrorMessage = "Status must be either 'Booked' or 'Cancelled'.")]
        //public string Status { get; set; } = "Booked";
    }
}
