using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public SeatService(AppDbContext context, IMapper mapper, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SeatDTO>> GetSeatsByRoute(int routeId)
        {
            try
            {
                _logger.Info($"Fetching seats for RouteId: {routeId}");

                var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == routeId && !r.IsDeleted);
                if (route == null)
                {
                    _logger.Warn($"Route ID {routeId} not found or marked as deleted.");
                    return Enumerable.Empty<SeatDTO>();
                }

                var seats = await _context.Seats
                    .Where(s => s.RouteId == routeId)
                    .ToListAsync();

                _logger.Info($"Total seats found for RouteId {routeId}: {seats.Count}");
                return _mapper.Map<IEnumerable<SeatDTO>>(seats);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching seats for RouteId {routeId}", ex);
                throw new Exception($"Error fetching seats: {ex.Message}");
            }
        }

        public async Task<List<Seat>> BookSeats(int routeId, int count)
        {
            try
            {
                _logger.Info($"Booking request received: RouteId={routeId}, RequestedSeats={count}");

                var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == routeId && !r.IsDeleted);
                if (route == null)
                {
                    _logger.Warn($"Route ID {routeId} not found or marked as deleted.");
                    return new List<Seat>();
                }

                var availableSeats = await _context.Seats
                    .Where(s => s.RouteId == routeId && !s.IsBooked)
                    .Take(count)
                    .ToListAsync();

                if (availableSeats.Count < count)
                {
                    _logger.Warn($"Insufficient available seats: Requested={count}, Available={availableSeats.Count}");
                    return new List<Seat>();
                }

                foreach (var seat in availableSeats)
                {
                    seat.IsBooked = true;
                }

                await _context.SaveChangesAsync();

                _logger.Info($"{count} seat(s) booked successfully for RouteId={routeId}");
                return availableSeats;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error booking seats for RouteId {routeId}", ex);
                throw new Exception($"Error booking seats: {ex.Message}");
            }
        }
    }
}
