using FastX_BusTicketBooking.API.Models.DTOs;

namespace FastX_BusTicketBooking.API.Services.Interfaces
{
    public interface IRefundService
    {
        public Task<string> ProcessRefund(RefundDTO refundDTO);
        public Task<IEnumerable<RefundDTO>> GetAllRefunds();
        public Task<RefundDTO?> GetRefundByBookingId(int bookingId);
    }
}
