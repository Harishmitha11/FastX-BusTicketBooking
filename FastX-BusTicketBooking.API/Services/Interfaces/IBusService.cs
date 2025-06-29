using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface IBusService
    {
        public Task<IEnumerable<BusDTO>> GetAllBuses();
        public Task<BusDTO?> GetBusById(int id);
        public Task<string> AddBus(BusDTO busDTO);
        public Task<string> UpdateBus(int id, BusDTO busDTO);   
        public Task<string> DeleteBus(int id);
    }
}
