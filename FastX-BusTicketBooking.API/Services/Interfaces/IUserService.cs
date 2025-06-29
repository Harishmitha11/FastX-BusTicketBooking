using FastX_BusTicketBooking.API.Models.DTOs;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDTO>> GetAllUsers();
        public Task<UserDTO?> GetUserById(int id);
        public Task<string> DeleteUser(int id);
    }
}
