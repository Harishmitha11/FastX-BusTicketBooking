using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Implementations;
using FastX_BusTicketBooking.API.Services.Interfaces;
using log4net;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastX_BusTicketBooking.Services
{
    [TestFixture]
    public class RefundServiceTests
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private ILog _logger;
        private RefundService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RefundDTO, Refund>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _logger = Mock.Of<ILog>();
            _service = new RefundService(_context, _mapper, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task When_ValidRefund_Should_ProcessSuccessfully()
        {
            var booking = new Booking
            {
                BookingId = 1,
                UserId = 10,
                RouteId = 2,
                BookingDate = DateTime.Now,
                NoOfSeats = 2,
                TotalFare = 400,
                Status = "Cancelled"
            };

            var user = new User
            {
                UserId = 99,
                Name = "Admin",
                Email = "admin@test.com",
                Gender = "Male",
                Address = "Test",
                ContactNumber = "9876543210",
                PasswordHash = "password",
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            };

            _context.Users.Add(user);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var dto = new RefundDTO
            {
                BookingId = 1,
                RefundAmount = 200,
                ProcessedBy = 99
            };

            var result = await _service.ProcessRefund(dto);

            Assert.That(result, Is.EqualTo("Refund processed successfully."));
        }

        [Test]
        public async Task When_InvalidBooking_Should_ReturnError()
        {
            var dto = new RefundDTO
            {
                BookingId = 999,
                RefundAmount = 100,
                ProcessedBy = 1
            };

            var result = await _service.ProcessRefund(dto);

            Assert.That(result, Is.EqualTo("Invalid booking ID."));
        }

        [Test]
        public async Task When_RefundAlreadyExists_Should_ReturnError()
        {
            var booking = new Booking
            {
                BookingId = 2,
                UserId = 10,
                RouteId = 2,
                BookingDate = DateTime.Now,
                NoOfSeats = 2,
                TotalFare = 400,
                Status = "Cancelled"
            };

            var user = new User
            {
                UserId = 100,
                Name = "Admin",
                Email = "admin@test.com",
                Gender = "Male",
                Address = "Test",
                ContactNumber = "9876543210",
                PasswordHash = "password",
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            };

            var refund = new Refund
            {
                BookingId = 2,
                RefundAmount = 100,
                RefundDate = DateTime.Now,
                ProcessedBy = 100,
                ProcessedByUser = user
            };

            _context.Users.Add(user);
            _context.Bookings.Add(booking);
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            var dto = new RefundDTO
            {
                BookingId = 2,
                RefundAmount = 200,
                ProcessedBy = 100
            };

            var result = await _service.ProcessRefund(dto);

            Assert.That(result, Is.EqualTo("Refund already processed for this booking."));
        }

        [Test]
        public async Task When_NoCancellations_Should_ReturnNotApplicable()
        {
            var booking = new Booking
            {
                BookingId = 3,
                UserId = 15,
                RouteId = 5,
                BookingDate = DateTime.Now,
                NoOfSeats = 1,
                TotalFare = 250,
                Status = "Booked"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var dto = new RefundDTO
            {
                BookingId = 3,
                RefundAmount = 250,
                ProcessedBy = 1
            };

            var result = await _service.ProcessRefund(dto);

            Assert.That(result, Is.EqualTo("Refund not applicable. No booking or seats are cancelled."));
        }

        [Test]
        public async Task When_GetAllRefunds_Should_ReturnList()
        {
            // Arrange
            var role = new Role
            {
                RoleId = 1,
                RoleName = "Admin"
            };

            var user = new User
            {
                UserId = 1,
                Name = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "hashed",
                Gender = "Male",
                ContactNumber = "9876543210",
                Address = "Admin Address",
                RoleId = 1,
                Role = role,
                CreatedAt = DateTime.Now
            };

            var route = new Route
            {
                RouteId = 1,
                BusId = 1,
                Origin = "CityA",
                Destination = "CityB",
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(5),
                Fare = 200,
                IsDeleted = false
            };

            var booking = new Booking
            {
                BookingId = 1,
                UserId = 1,
                User = user,
                RouteId = 1,
                Route = route,
                BookingDate = DateTime.Now,
                NoOfSeats = 2,
                TotalFare = 400,
                Status = "Cancelled"
            };

            var refund1 = new Refund
            {
                RefundId = 1,
                BookingId = 1,
                Booking = booking,
                RefundAmount = 150,
                RefundDate = DateTime.Now,
                ProcessedBy = 1,
                ProcessedByUser = user
            };

            var refund2 = new Refund
            {
                RefundId = 2,
                BookingId = 1,
                Booking = booking,
                RefundAmount = 200,
                RefundDate = DateTime.Now,
                ProcessedBy = 1,
                ProcessedByUser = user
            };

            _context.Roles.Add(role);
            _context.Users.Add(user);
            _context.Routes.Add(route);
            _context.Bookings.Add(booking);
            _context.Refunds.AddRange(refund1, refund2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllRefunds();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }


        [Test]
        public async Task When_RefundExistsByBookingId_Should_ReturnRefund()
        {
            var user = new User
            {
                UserId = 101,
                Name = "Refund Admin",
                Email = "admin@refund.com",
                Gender = "Male",
                Address = "Address",
                ContactNumber = "9876543210",
                PasswordHash = "pwd",
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            };

            var booking = new Booking
            {
                BookingId = 6,
                UserId = 20,
                RouteId = 2,
                BookingDate = DateTime.Now,
                NoOfSeats = 2,
                TotalFare = 500,
                Status = "Cancelled"
            };

            var refund = new Refund
            {
                BookingId = 6,
                RefundAmount = 300,
                RefundDate = DateTime.Now,
                ProcessedBy = 101,
                ProcessedByUser = user,
                Booking = booking
            };

            _context.Users.Add(user);
            _context.Bookings.Add(booking);
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            var result = await _service.GetRefundByBookingId(6);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.BookingId, Is.EqualTo(6));
        }


        [Test]
        public async Task When_RefundNotExistsByBookingId_Should_ReturnNull()
        {
            var result = await _service.GetRefundByBookingId(999);

            Assert.That(result, Is.Null);
        }
    }
}