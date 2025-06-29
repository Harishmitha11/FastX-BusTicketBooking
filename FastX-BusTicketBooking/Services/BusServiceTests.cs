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
    public class BusServiceTests
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private ILog _logger;
        private BusService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BusDTO, Bus>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _logger = Mock.Of<ILog>();

            _service = new BusService(_context, _mapper, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
        [Test]
        public async Task When_AddBusIsCalledWithValidData_Should_AddBus()
        {
            var dto = new BusDTO
            {
                BusName = "TN Travels",
                BusNumber = "TN01AB1234",
                BusType = "AC",
                TotalSeats = 40,
                Amenities = "WiFi"
            };

            var result = await _service.AddBus(dto);

            Assert.That(result, Is.EqualTo("Bus added successfully."));
            Assert.That(await _context.Buses.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task When_AddBusWithExistingNumber_Should_ReturnDuplicateError()
        {
            var dto = new BusDTO
            {
                BusName = "TN Travels",
                BusNumber = "TN01AB1234",
                BusType = "AC",
                TotalSeats = 40,
                Amenities = "WiFi"
            };
            await _service.AddBus(dto);

            var duplicateResult = await _service.AddBus(dto);

            Assert.That(duplicateResult, Does.Contain("already exists"));
        }

        [Test]
        public async Task When_GetAllBuses_Should_ReturnBusList()
        {
            await _context.Buses.AddAsync(new Bus
            {
                BusName = "Test Bus",
                BusNumber = "TN01CD5678",
                BusType = "NonAC",
                TotalSeats = 35,
                Amenities = "Fan"
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllBuses();

            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task When_GetBusByIdExists_Should_ReturnBus()
        {
            var bus = new Bus
            {
                BusName = "Test Bus",
                BusNumber = "TN09XY9999",
                BusType = "Sleeper",
                TotalSeats = 50,
                Amenities = "AC, Charger"
            };
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            var result = await _service.GetBusById(bus.BusId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.BusNumber, Is.EqualTo("TN09XY9999"));
        }

        [Test]
        public async Task When_GetBusByIdNotExists_Should_ReturnNull()
        {
            var result = await _service.GetBusById(1000);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task When_UpdateBusIsCalled_Should_UpdateSuccessfully()
        {
            var bus = new Bus
            {
                BusName = "KPN",
                BusNumber = "TN10ZZ1010",
                BusType = "AC",
                TotalSeats = 40,
                Amenities = "Charger"
            };
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            var updateDto = new BusDTO
            {
                BusId = bus.BusId,
                BusName = "KPN Travels",
                BusNumber = "TN10ZZ1010",
                BusType = "AC Sleeper",
                TotalSeats = 45,
                Amenities = "Charger, AC"
            };

            var result = await _service.UpdateBus(bus.BusId, updateDto);

            Assert.That(result, Is.EqualTo("Bus updated successfully."));
        }

        [Test]
        public async Task When_UpdateBusWithInvalidId_Should_ReturnError()
        {
            var updateDto = new BusDTO
            {
                BusId = 999,
                BusName = "XYZ",
                BusNumber = "TN11XY1111",
                BusType = "NonAC",
                TotalSeats = 30,
                Amenities = "None"
            };

            var result = await _service.UpdateBus(999, updateDto);

            Assert.That(result, Is.EqualTo("Bus not found to update."));
        }

        [Test]
        public async Task When_DeleteBusIsCalled_Should_MarkAsDeleted()
        {
            var bus = new Bus
            {
                BusName = "VRL",
                BusNumber = "TN22VR2222",
                BusType = "Luxury",
                TotalSeats = 40,
                Amenities = "AC"
            };
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteBus(bus.BusId);

            Assert.That(result, Is.EqualTo("Bus soft-deleted successfully."));
            var softDeleted = await _context.Buses.FindAsync(bus.BusId);
            Assert.That(softDeleted!.IsDeleted, Is.True);
        }

        [Test]
        public async Task When_DeleteBusWithInvalidId_Should_ReturnError()
        {
            var result = await _service.DeleteBus(888);

            Assert.That(result, Is.EqualTo("Bus not found to delete."));
        }
    }
}

