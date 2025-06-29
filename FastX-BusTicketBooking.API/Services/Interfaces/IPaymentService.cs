using FastX_BusTicketBooking.API.Models.DTOs;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<string> ProcessPayment(PaymentDTO paymentDTO);
        public Task<IEnumerable<PaymentDTO>> GetAllPayments();
        public Task<PaymentDTO?> GetPaymentByBookingId(int bookingId);
    }
}
