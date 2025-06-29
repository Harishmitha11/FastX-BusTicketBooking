using FastX_BusTicketBooking.API.Models.DTOs;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface IBookingService
    {
        public Task<string> BookTicket(BookingRequestDTO bookingDTO);
        public Task<IEnumerable<BookingResponseDTO>> GetAllBookings();
        public Task<BookingResponseDTO?> GetBookingById(int id);
        public Task<string> CancelBooking(int id);
        public Task<IEnumerable<BookingResponseDTO>> GetBookingsByUser(int userId);
        public Task<string> CancelSelectedSeats(CancelSeatsDTO dto);
    }
}
