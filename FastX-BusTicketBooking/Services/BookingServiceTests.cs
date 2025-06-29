using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Implementations;
using FastX_BusTicketBooking.API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace FastX_BusTicketBooking.Services
{
    [TestFixture]
    public class BookingServiceTests
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private BookingService _bookingService;
        private Mock<ISeatService> _mockSeatService;
        private Mock<ILog> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Booking, BookingResponseDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _mockSeatService = new Mock<ISeatService>();
            _mockLogger = new Mock<ILog>();
            _bookingService = new BookingService(_context, _mapper, _mockSeatService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task When_ValidBooking_Should_BookTicket()
        {
            var route = new Route
            {
                RouteId = 1,
                BusId = 1,
                Fare = 500,
                Origin = "CityA",
                Destination = "CityB",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            _context.Routes.Add(route);
            _context.SaveChanges();

            _mockSeatService.Setup(s => s.BookSeats(1, 2))
                .ReturnsAsync(new List<Seat> {
                    new Seat { SeatId = 1, RouteId = 1, SeatNumber = "S1" },
                    new Seat { SeatId = 2, RouteId = 1, SeatNumber = "S2" }
                });

            var dto = new BookingRequestDTO { UserId = 101, RouteId = 1, NoOfSeats = 2 };
            var result = await _bookingService.BookTicket(dto);

            Assert.That(result, Is.EqualTo("Booking successful."));
        }

        [Test]
        public async Task When_InvalidRoute_Should_ReturnError()
        {
            var dto = new BookingRequestDTO { UserId = 10, RouteId = 9999, NoOfSeats = 1 };
            var result = await _bookingService.BookTicket(dto);

            Assert.That(result, Is.EqualTo("Invalid route selected."));
        }

        [Test]
        public async Task When_InsufficientSeats_Should_ReturnError()
        {
            var route = new Route
            {
                RouteId = 2,
                BusId = 2,
                Fare = 300,
                Origin = "CityX",
                Destination = "CityY",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(1)
            };
            _context.Routes.Add(route);
            _context.SaveChanges();

            _mockSeatService.Setup(s => s.BookSeats(2, 5)).ReturnsAsync(new List<Seat>());

            var dto = new BookingRequestDTO { UserId = 102, RouteId = 2, NoOfSeats = 5 };
            var result = await _bookingService.BookTicket(dto);

            Assert.That(result, Is.EqualTo("Not enough seats available."));
        }

        [Test]
        public async Task When_GetBookingByIdExists_Should_ReturnBooking()
        {
            var booking = new Booking
            {
                BookingId = 10,
                UserId = 201,
                RouteId = 3,
                BookingDate = DateTime.Now,
                NoOfSeats = 1,
                TotalFare = 300,
                Status = "Booked"
            };
            var route = new Route
            {
                RouteId = 3,
                BusId = 3,
                Fare = 300,
                Origin = "From",
                Destination = "To",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            var user = new User { UserId = 201, Name = "Test", Email = "test@test.com", Gender = "Male", PasswordHash = "hash", ContactNumber = "9876543210", Address = "Address", RoleId = 1 };

            _context.Users.Add(user);
            _context.Routes.Add(route);
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var result = await _bookingService.GetBookingById(10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BookingId, Is.EqualTo(10));
        }

        [Test]
        public async Task When_FetchAllBookings_Should_ReturnList()
        {
            _context.Bookings.Add(new Booking { BookingId = 11, UserId = 301, RouteId = 4, BookingDate = DateTime.Now, NoOfSeats = 1, TotalFare = 250, Status = "Booked" });
            _context.Routes.Add(new Route { RouteId = 4, BusId = 4, Fare = 250, Origin = "From", Destination = "To", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(2) });
            _context.Users.Add(new User { UserId = 301, Name = "U", Email = "u@mail.com", Gender = "Female", PasswordHash = "p", ContactNumber = "9999999999", Address = "Addr", RoleId = 1 });
            _context.SaveChanges();

            var result = await _bookingService.GetAllBookings();

            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task When_GetBookingsByUser_Should_ReturnTheirBookings()
        {
            var booking = new Booking
            {
                BookingId = 20,
                UserId = 401,
                RouteId = 5,
                BookingDate = DateTime.Now,
                NoOfSeats = 2,
                TotalFare = 600,
                Status = "Booked"
            };
            var route = new Route
            {
                RouteId = 5,
                BusId = 5,
                Fare = 300,
                Origin = "City1",
                Destination = "City2",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            _context.Bookings.Add(booking);
            _context.Routes.Add(route);
            _context.SaveChanges();

            var result = await _bookingService.GetBookingsByUser(401);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task When_BookingNotFoundForCancellation_Should_ReturnNull()
        {
            var result = await _bookingService.CancelBooking(999); // non-existent ID
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task When_CancelSelectedSeats_Should_UpdateIsCancelled()
        {
            var route = new Route
            {
                RouteId = 6,
                BusId = 6,
                Fare = 200,
                Origin = "A",
                Destination = "B",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            var seat = new Seat { SeatId = 300, RouteId = 6, SeatNumber = "S10", IsBooked = true };
            var booking = new Booking { BookingId = 300, UserId = 50, RouteId = 6, NoOfSeats = 1, TotalFare = 200, Status = "Booked" };
            var bookingSeat = new BookingSeat { Id = 1, BookingId = 300, SeatId = 300, IsCancelled = false };

            _context.Routes.Add(route);
            _context.Seats.Add(seat);
            _context.Bookings.Add(booking);
            _context.BookingSeats.Add(bookingSeat);
            _context.SaveChanges();

            var dto = new CancelSeatsDTO { BookingId = 300, SeatIds = new List<int> { 300 } };
            var result = await _bookingService.CancelSelectedSeats(dto);

            Assert.That(result, Is.EqualTo("1 seat(s) cancelled successfully."));
        }

        [Test]
        public async Task When_BookingIdIsInvalidInCancelSeats_Should_ReturnError()
        {
            var dto = new CancelSeatsDTO
            {
                BookingId = 999,
                SeatIds = new List<int> { 1 }
            };

            var result = await _bookingService.CancelSelectedSeats(dto);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task When_SeatAlreadyCancelled_Should_ReturnPartialError()
        {
            var route = new Route
            {
                RouteId = 7,
                BusId = 7,
                Fare = 100,
                Origin = "East",
                Destination = "West",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            _context.Routes.Add(route);

            var seat = new Seat { SeatId = 400, RouteId = 7, SeatNumber = "S12", IsBooked = false };
            _context.Seats.Add(seat);

            var booking = new Booking
            {
                BookingId = 400,
                UserId = 77,
                RouteId = 7,
                NoOfSeats = 1,
                TotalFare = 100,
                Status = "Booked"
            };
            _context.Bookings.Add(booking);

            var bookingSeat = new BookingSeat
            {
                Id = 2,
                BookingId = 400,
                SeatId = 400,
                IsCancelled = true 
            };
            _context.BookingSeats.Add(bookingSeat);

            _context.SaveChanges();

            var dto = new CancelSeatsDTO
            {
                BookingId = 400,
                SeatIds = new List<int> { 400 }
            };

            var result = await _bookingService.CancelSelectedSeats(dto);
            Assert.That(result, Is.EqualTo("Some seat IDs are invalid or already cancelled."));
        }
    }
}
