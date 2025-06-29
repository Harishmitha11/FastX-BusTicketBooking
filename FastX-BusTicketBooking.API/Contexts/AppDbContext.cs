using Microsoft.EntityFrameworkCore;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Implementations;

namespace FastX_BusTicketBooking.API.Contexts
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Models.Entities.Route> Routes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Payment> Payments { get; set; } 




    }
}
