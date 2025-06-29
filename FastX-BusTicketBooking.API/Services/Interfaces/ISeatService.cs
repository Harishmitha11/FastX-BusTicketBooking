using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface ISeatService
    {
        public Task<IEnumerable<SeatDTO>> GetSeatsByRoute(int routeId);
        public Task<List<Seat>> BookSeats(int routeId, int count);
    }
}
