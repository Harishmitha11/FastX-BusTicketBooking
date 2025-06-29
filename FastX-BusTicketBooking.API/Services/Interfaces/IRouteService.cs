using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface IRouteService
    {
        public Task<IEnumerable<Models.Entities.Route>> GetAllRoutes();
        public Task<Models.Entities.Route?> GetRouteById(int id);
        public Task<string> AddRoute(RouteDTO routeDTO);
        public Task<string> UpdateRoute(int id, RouteDTO routeDTO);
        public Task<string> DeleteRoute(int id);
        public Task<IEnumerable<RouteDTO>> SearchRoutes(string origin, string destination, DateTime travelDate);

    }
}
