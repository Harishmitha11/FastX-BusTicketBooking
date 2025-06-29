using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;
        private readonly ILog _logger;

        public BookingService(AppDbContext context, IMapper mapper, ISeatService seatService, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _seatService = seatService;
            _logger = logger;
        }

        public async Task<string> BookTicket(BookingRequestDTO bookingDTO)
        {
            try
            {
                _logger.Info($"Booking request: UserId={bookingDTO.UserId}, RouteId={bookingDTO.RouteId}, Seats={bookingDTO.NoOfSeats}");

                var route = await _context.Routes.FirstOrDefaultAsync(r => r.RouteId == bookingDTO.RouteId && !r.IsDeleted);
                if (route == null)
                {
                    _logger.Warn($"Invalid route ID: {bookingDTO.RouteId}");
                    return "Invalid route selected.";
                }

                var bookedSeats = await _seatService.BookSeats(bookingDTO.RouteId, bookingDTO.NoOfSeats);
                if (bookedSeats == null || bookedSeats.Count < bookingDTO.NoOfSeats)
                {
                    _logger.Warn($"Insufficient seats for RouteId={bookingDTO.RouteId}");
                    return "Not enough seats available.";
                }

                var booking = new Booking
                {
                    UserId = bookingDTO.UserId,
                    RouteId = bookingDTO.RouteId,
                    BookingDate = DateTime.Now,
                    NoOfSeats = bookingDTO.NoOfSeats,
                    TotalFare = bookingDTO.NoOfSeats * route.Fare,
                    Status = "Booked"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                foreach (var seat in bookedSeats)
                {
                    _context.BookingSeats.Add(new BookingSeat
                    {
                        BookingId = booking.BookingId,
                        SeatId = seat.SeatId,
                        IsCancelled = false
                    });
                }

                await _context.SaveChangesAsync();
                _logger.Info($"Booking successful: BookingId={booking.BookingId}");
                return "Booking successful.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error while booking ticket", ex);
                return $"Error: {ex.Message}";
            }
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllBookings()
        {
            try
            {
                _logger.Info("Fetching all bookings");

                var bookings = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Route)
                    .ToListAsync();

                _logger.Info($"Retrieved {bookings.Count} bookings");
                return _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all bookings", ex);
                throw new Exception($"Error fetching bookings: {ex.Message}");
            }
        }

        public async Task<BookingResponseDTO?> GetBookingById(int id)
        {
            try
            {
                _logger.Info($"Fetching booking ID: {id}");

                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Route)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                {
                    _logger.Warn($"Booking not found: ID={id}");
                    return null;
                }

                return _mapper.Map<BookingResponseDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching booking by ID={id}", ex);
                throw new Exception($"Error fetching booking: {ex.Message}");
            }
        }

        public async Task<string> CancelBooking(int id)
        {
            try
            {
                _logger.Info($"Attempting to cancel booking ID: {id}");

                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    _logger.Warn($"Booking not found: ID={id}");
                    return null;
                }

                booking.Status = "Cancelled";
                await _context.SaveChangesAsync();

                _logger.Info($"Booking cancelled: ID={id}");
                return "Booking cancelled.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error during cancellation", ex);
                return $"Error: {ex.Message}";
            }
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetBookingsByUser(int userId)
        {
            try
            {
                _logger.Info($"Fetching bookings for UserId: {userId}");

                var bookings = await _context.Bookings
                    .Include(b => b.Route)
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                _logger.Info($"User {userId} has {bookings.Count} bookings");
                return _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bookings for user {userId}", ex);
                throw new Exception($"Error fetching bookings: {ex.Message}");
            }
        }

        public async Task<string> CancelSelectedSeats(CancelSeatsDTO dto)
        {
            try
            {
                _logger.Info($"Cancelling selected seats for BookingId={dto.BookingId}");
                var booking = await _context.Bookings.FindAsync(dto.BookingId);
                if (booking == null)
                {
                    _logger.Warn($"Booking not found for ID: {dto.BookingId}");
                    return null;
                }


                var bookingSeats = await _context.BookingSeats
                    .Where(bs => bs.BookingId == dto.BookingId && dto.SeatIds.Contains(bs.SeatId) && !bs.IsCancelled)
                    .Include(bs => bs.Seat)
                    .ToListAsync();

                if (bookingSeats.Count != dto.SeatIds.Count)
                {
                    _logger.Warn("Some seats were invalid or already cancelled.");
                    return "Some seat IDs are invalid or already cancelled.";
                }

                foreach (var bs in bookingSeats)
                {
                    bs.IsCancelled = true;
                    bs.Seat.IsBooked = false;
                }

                await _context.SaveChangesAsync();
                _logger.Info($"Cancelled {bookingSeats.Count} seat(s) for BookingId={dto.BookingId}");
                return $"{bookingSeats.Count} seat(s) cancelled successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error while cancelling selected seats", ex);
                return $"Error: {ex.Message}";
            }
        }
    }
}
