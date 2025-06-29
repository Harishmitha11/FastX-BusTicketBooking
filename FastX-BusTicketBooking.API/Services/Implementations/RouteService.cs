using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public RouteService(AppDbContext context, IMapper mapper, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Models.Entities.Route>> GetAllRoutes()
        {
            try
            {
                _logger.Info("Fetching all routes...");
                var routes = await _context.Routes.Where(r => !r.IsDeleted).ToListAsync();
                _logger.Info($"Total routes retrieved: {routes.Count}");
                return routes;
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching routes", ex);
                throw new Exception($"Error fetching routes: {ex.Message}");
            }
        }

        public async Task<Models.Entities.Route?> GetRouteById(int id)
        {
            try
            {
                _logger.Info($"Fetching route by ID: {id}");
                var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == id && !r.IsDeleted);

                if (route == null)
                {
                    _logger.Warn($"Route not found with ID: {id}");
                    return null;
                }

                _logger.Info($"Route found: RouteId={id}");
                return route;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching route by ID: {id}", ex);
                throw new Exception($"Error fetching route: {ex.Message}");
            }
        }

        public async Task<string> AddRoute(RouteDTO routeDTO)
        {
            try
            {
                _logger.Info($"Attempting to add route: Origin={routeDTO.Origin}, Destination={routeDTO.Destination}");

                var isDuplicate = await _context.Routes.AnyAsync(r =>
                    r.Origin.ToLower() == routeDTO.Origin.ToLower() &&
                    r.Destination.ToLower() == routeDTO.Destination.ToLower() &&
                    r.BusId == routeDTO.BusId &&
                    r.DepartureTime.Date == routeDTO.DepartureTime.Date &&
                    !r.IsDeleted);

                if (isDuplicate)
                {
                    _logger.Warn($"Duplicate route found for Origin={routeDTO.Origin}, Destination={routeDTO.Destination}, BusId={routeDTO.BusId}");
                    return "A route with the same origin, destination, bus, and date already exists.";
                }

                var route = _mapper.Map<Models.Entities.Route>(routeDTO);
                _context.Routes.Add(route);
                await _context.SaveChangesAsync();

                var bus = await _context.Buses.FindAsync(routeDTO.BusId);
                if (bus == null)
                {
                    _logger.Warn($"Bus not found for ID: {routeDTO.BusId}");
                    return "Bus not found.";
                }

                var seats = new List<Seat>();
                for (int i = 1; i <= bus.TotalSeats; i++)
                {
                    seats.Add(new Seat
                    {
                        RouteId = route.RouteId,
                        SeatNumber = $"S{i}",
                        IsBooked = false
                    });
                }

                _context.Seats.AddRange(seats);
                await _context.SaveChangesAsync();

                _logger.Info($"Route added successfully: RouteId={route.RouteId}");
                return "Route Added Successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error adding route", ex);
                throw new Exception($"Error adding route: {ex.Message}");
            }
        }

        public async Task<string> UpdateRoute(int id, RouteDTO routeDTO)
        {
            try
            {
                _logger.Info($"Attempting to update route: RouteId={id}");

                var route = await _context.Routes.FindAsync(id);
                if (route == null)
                {
                    _logger.Warn($"Route not found to update: RouteId={id}");
                    return "Route not found.";
                }

                _mapper.Map(routeDTO, route);
                await _context.SaveChangesAsync();

                _logger.Info($"Route updated successfully: RouteId={id}");
                return "Route Updated Successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating route: RouteId={id}", ex);
                throw new Exception($"Error updating route: {ex.Message}");
            }
        }

        public async Task<string> DeleteRoute(int id)
        {
            try
            {
                _logger.Info($"Attempting to delete route: RouteId={id}");

                var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == id && !r.IsDeleted);
                if (route == null)
                {
                    _logger.Warn($"Route not found to delete: RouteId={id}");
                    return "Route not found.";
                }

                route.IsDeleted = true;
                await _context.SaveChangesAsync();

                _logger.Info($"Route deleted successfully: RouteId={id}");
                return "Route soft-deleted Successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting route: RouteId={id}", ex);
                throw new Exception($"Error deleting route: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RouteDTO>> SearchRoutes(string origin, string destination, DateTime travelDate)
        {
            try
            {
                _logger.Info($"Searching routes: Origin={origin}, Destination={destination}, Date={travelDate:yyyy-MM-dd}");

                var routes = await _context.Routes
                    .Include(r => r.Bus)
                    .Where(r => r.Origin == origin &&
                                r.Destination == destination &&
                                r.DepartureTime.Date == travelDate.Date &&
                                !r.IsDeleted)
                    .ToListAsync();

                _logger.Info($"Search complete. Routes found: {routes.Count}");
                return _mapper.Map<IEnumerable<RouteDTO>>(routes);
            }
            catch (Exception ex)
            {
                _logger.Error("Error during route search", ex);
                throw new Exception($"Error searching routes: {ex.Message}");
            }
        }
    }
}
