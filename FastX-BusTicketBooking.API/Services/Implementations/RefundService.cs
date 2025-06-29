using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class RefundService : IRefundService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public RefundService(AppDbContext context, IMapper mapper, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> ProcessRefund(RefundDTO refundDTO)
        {
            try
            {
                _logger.Info($"Refund request received: BookingId={refundDTO.BookingId}, Amount={refundDTO.RefundAmount}, ProcessedBy={refundDTO.ProcessedBy}");

                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == refundDTO.BookingId);
                if (booking == null)
                {
                    _logger.Warn($"Refund failed: Invalid BookingId={refundDTO.BookingId}");
                    return "Invalid booking ID.";
                }

                var existingRefund = await _context.Refunds.FirstOrDefaultAsync(r => r.BookingId == refundDTO.BookingId);
                if (existingRefund != null)
                {
                    _logger.Warn($"Refund already processed for BookingId={refundDTO.BookingId}");
                    return "Refund already processed for this booking.";
                }

                bool isBookingCancelled = booking.Status == "Cancelled";

                var cancelledSeats = await _context.BookingSeats
                    .Where(bs => bs.BookingId == refundDTO.BookingId && bs.IsCancelled)
                    .ToListAsync();

                if (!isBookingCancelled && !cancelledSeats.Any())
                {
                    _logger.Warn($"Refund not applicable: No cancellation for BookingId={refundDTO.BookingId}");
                    return "Refund not applicable. No booking or seats are cancelled.";
                }

                var refund = _mapper.Map<Refund>(refundDTO);
                refund.RefundDate = DateTime.Now;

                _context.Refunds.Add(refund);
                await _context.SaveChangesAsync();

                _logger.Info($"Refund processed successfully: RefundId={refund.RefundId}");
                return "Refund processed successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error during refund processing", ex);
                throw new Exception($"Error during refund processing: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RefundDTO>> GetAllRefunds()
        {
            try
            {
                _logger.Info("Fetching all refund records...");

                var refunds = await _context.Refunds
                    .Include(r => r.Booking)
                    .Include(r => r.ProcessedByUser)
                    .ToListAsync();

                _logger.Info($"Total refunds found: {refunds.Count}");
                return _mapper.Map<IEnumerable<RefundDTO>>(refunds);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all refunds", ex);
                throw new Exception($"Error fetching refund list: {ex.Message}");
            }
        }

        public async Task<RefundDTO?> GetRefundByBookingId(int bookingId)
        {
            try
            {
                _logger.Info($"Fetching refund by BookingId: {bookingId}");

                var refund = await _context.Refunds
                    .Include(r => r.Booking)
                    .Include(r => r.ProcessedByUser)
                    .FirstOrDefaultAsync(r => r.BookingId == bookingId);

                if (refund == null)
                {
                    _logger.Warn($"Refund not found for BookingId: {bookingId}");
                    return null;
                }

                _logger.Info($"Refund found: RefundId={refund.RefundId}");
                return _mapper.Map<RefundDTO>(refund);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching refund by BookingId={bookingId}", ex);
                throw new Exception($"Error fetching refund: {ex.Message}");
            }
        }
    }
}
