using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Implementations;
using log4net;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FastX_BusTicketBooking.Services
{
    [TestFixture]
    public class SeatServiceTests
    {
        private AppDbContext _context;
        private ISeatService _seatService;
        private IMapper _mapper;
        private Mock<ILog> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SeatServiceTestDB")
                .Options;

            _context = new AppDbContext(options);
            _mockLogger = new Mock<ILog>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Seat, SeatDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _seatService = new SeatService(_context, _mapper, _mockLogger.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            
            _context.Dispose();
        }

        [Test]
        public async Task When_SeatsExistForRoute_Should_ReturnSeatList()
        {
            var route = new Route { RouteId = 1, Origin = "A", Destination = "B", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(2), Fare = 200, BusId = 1 };
            _context.Routes.Add(route);
            _context.Seats.AddRange(
                new Seat { SeatId = 1, RouteId = 1, SeatNumber = "S1", IsBooked = false },
                new Seat { SeatId = 2, RouteId = 1, SeatNumber = "S2", IsBooked = false }
            );
            await _context.SaveChangesAsync();

            var result = await _seatService.GetSeatsByRoute(1);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task When_RouteDoesNotExistIn_GetSeatsByRoute_Should_ReturnEmptyList()
        {
            var result = await _seatService.GetSeatsByRoute(999);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task When_EnoughSeatsAvailable_Should_BookSeats()
        {
            var route = new Route { RouteId = 2, Origin = "X", Destination = "Y", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(1), Fare = 100, BusId = 2 };
            _context.Routes.Add(route);
            _context.Seats.AddRange(
                new Seat { SeatId = 3, RouteId = 2, SeatNumber = "S3", IsBooked = false },
                new Seat { SeatId = 4, RouteId = 2, SeatNumber = "S4", IsBooked = false }
            );
            await _context.SaveChangesAsync();

            var result = await _seatService.BookSeats(2, 2);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(s => s.IsBooked), Is.True);
        }

        [Test]
        public async Task When_InsufficientSeats_Should_ReturnEmptyList()
        {
            var route = new Route { RouteId = 3, Origin = "C", Destination = "D", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(1), Fare = 150, BusId = 3 };
            _context.Routes.Add(route);
            _context.Seats.Add(new Seat { SeatId = 5, RouteId = 3, SeatNumber = "S5", IsBooked = false });
            await _context.SaveChangesAsync();

            var result = await _seatService.BookSeats(3, 2); // Requesting 2 but only 1 available

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task When_RouteInvalidForBooking_Should_ReturnEmptyList()
        {
            var result = await _seatService.BookSeats(999, 1);

            Assert.That(result, Is.Empty);
        }
    }
}
