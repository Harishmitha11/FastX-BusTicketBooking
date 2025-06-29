using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Implementations;
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
    public class PaymentServiceTests
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private ILog _logger;
        private PaymentService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PaymentDTO, Payment>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _logger = Mock.Of<ILog>();
            _service = new PaymentService(_context, _mapper, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task When_ValidPayment_Should_ProcessSuccessfully()
        {
            var booking = new Booking
            {
                BookingId = 1,
                UserId = 101,
                RouteId = 5,
                BookingDate = DateTime.Now,
                NoOfSeats = 2,
                TotalFare = 500,
                Status = "Booked"
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var dto = new PaymentDTO
            {
                BookingId = 1,
                Amount = 500,
                PaymentMethod = "UPI"
            };

            var result = await _service.ProcessPayment(dto);

            Assert.That(result, Is.EqualTo("Payment processed successfully."));
        }

        [Test]
        public async Task When_BookingIdIsInvalid_Should_ReturnError()
        {
            var dto = new PaymentDTO
            {
                BookingId = 999,
                Amount = 200,
                PaymentMethod = "Cash"
            };

            var result = await _service.ProcessPayment(dto);

            Assert.That(result, Is.EqualTo("Invalid booking ID."));
        }

        [Test]
        public async Task When_DuplicatePaymentAttempt_Should_ReturnError()
        {
            var booking = new Booking
            {
                BookingId = 2,
                UserId = 101,
                RouteId = 7,
                BookingDate = DateTime.Now,
                NoOfSeats = 1,
                TotalFare = 250,
                Status = "Booked"
            };
            _context.Bookings.Add(booking);

            var payment = new Payment
            {
                BookingId = 2,
                Amount = 250,
                PaymentMethod = "Card",
                Status = "Success",
                PaymentDate = DateTime.Now
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var dto = new PaymentDTO
            {
                BookingId = 2,
                Amount = 250,
                PaymentMethod = "Card"
            };

            var result = await _service.ProcessPayment(dto);

            Assert.That(result, Is.EqualTo("Payment already exists for this booking."));
        }

        [Test]
        public async Task When_GetAllPayments_Should_ReturnList()
        {
            _context.Payments.AddRange(new[]
            {
                new Payment { BookingId = 3, Amount = 300, PaymentMethod = "UPI", PaymentDate = DateTime.Now, Status = "Success" },
                new Payment { BookingId = 4, Amount = 400, PaymentMethod = "Card", PaymentDate = DateTime.Now, Status = "Success" }
            });

            await _context.SaveChangesAsync();

            var result = await _service.GetAllPayments();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task When_PaymentExistsByBookingId_Should_ReturnPayment()
        {
            var payment = new Payment
            {
                BookingId = 5,
                Amount = 600,
                PaymentMethod = "NetBanking",
                PaymentDate = DateTime.Now,
                Status = "Success"
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = await _service.GetPaymentByBookingId(5);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.BookingId, Is.EqualTo(5));
        }

        [Test]
        public async Task When_PaymentNotExistsByBookingId_Should_ReturnNull()
        {
            var result = await _service.GetPaymentByBookingId(9999);

            Assert.That(result, Is.Null);
        }
    }
}
