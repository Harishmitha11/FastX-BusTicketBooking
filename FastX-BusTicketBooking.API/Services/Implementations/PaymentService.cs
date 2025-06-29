using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public PaymentService(AppDbContext context, IMapper mapper, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> ProcessPayment(PaymentDTO paymentDTO)
        {
            try
            {
                _logger.Info($"Processing payment: BookingId={paymentDTO.BookingId}, Amount={paymentDTO.Amount}");

                if (!await _context.Bookings.AnyAsync(b => b.BookingId == paymentDTO.BookingId))
                {
                    _logger.Warn($"Invalid booking ID: {paymentDTO.BookingId}");
                    return "Invalid booking ID.";
                }

                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == paymentDTO.BookingId);

                if (existingPayment != null)
                {
                    _logger.Warn($"Duplicate payment attempt: BookingId={paymentDTO.BookingId}");
                    return "Payment already exists for this booking.";
                }

                var payment = new Payment
                {
                    BookingId = paymentDTO.BookingId,
                    Amount = paymentDTO.Amount,
                    PaymentMethod = paymentDTO.PaymentMethod,
                    Status = "Success",
                    PaymentDate = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.Info($"Payment successful: PaymentId={payment.PaymentId}");
                return "Payment processed successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error processing payment", ex);
                return $"Error processing payment: {ex.Message}";
            }
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllPayments()
        {
            try
            {
                _logger.Info("Fetching all payments...");
                var payments = await _context.Payments.ToListAsync();
                _logger.Info($"Fetched {payments.Count} payments.");
                return _mapper.Map<IEnumerable<PaymentDTO>>(payments);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching payments", ex);
                throw new Exception($"Error fetching payments: {ex.Message}");
            }
        }

        public async Task<PaymentDTO?> GetPaymentByBookingId(int bookingId)
        {
            try
            {
                _logger.Info($"Fetching payment by BookingId={bookingId}");

                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);

                if (payment == null)
                {
                    _logger.Warn($"No payment found for BookingId={bookingId}");
                    return null;
                }

                _logger.Info($"Payment found: PaymentId={payment.PaymentId}");
                return _mapper.Map<PaymentDTO>(payment);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching payment by BookingId={bookingId}", ex);
                throw new Exception($"Error fetching payment: {ex.Message}");
            }
        }
    }
}
