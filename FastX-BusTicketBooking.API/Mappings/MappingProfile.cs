using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using AutoMapper;


namespace FastX_BusTicketBooking.API.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<BusDTO, Bus>().ReverseMap();
            CreateMap<RouteDTO, Models.Entities.Route>().ReverseMap();
            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<BookingRequestDTO, Booking>();
            CreateMap<Booking, BookingResponseDTO>();
            CreateMap<Refund, RefundDTO>().ReverseMap();
            CreateMap<Seat, SeatDTO>().ReverseMap();
            CreateMap<Payment, PaymentDTO>().ReverseMap();


        }

    }
}
