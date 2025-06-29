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
    public class RouteServiceTests
    {
        private AppDbContext _context;
        private RouteService _routeService;
        private IMapper _mapper;
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
                cfg.CreateMap<Route, RouteDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _mockLogger = new Mock<ILog>();
            _routeService = new RouteService(_context, _mapper, _mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task When_GetAllRoutes_Should_ReturnList()
        {
            var route1 = new Route { RouteId = 1, BusId = 1, Origin = "CityA", Destination = "CityB", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(2), Fare = 100, IsDeleted = false };
            var route2 = new Route { RouteId = 2, BusId = 2, Origin = "CityC", Destination = "CityD", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(3), Fare = 150, IsDeleted = false };

            _context.Routes.AddRange(route1, route2);
            await _context.SaveChangesAsync();

            var result = await _routeService.GetAllRoutes();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task When_GetRouteByIdValid_Should_ReturnRoute()
        {
            var route = new Route
            {
                RouteId = 10,
                BusId = 3,
                Origin = "Chennai",
                Destination = "Madurai",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(6),
                Fare = 300,
                IsDeleted = false
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            var result = await _routeService.GetRouteById(10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.RouteId, Is.EqualTo(10));
        }

        [Test]
        public async Task When_RouteIdInvalid_Should_ReturnNull()
        {
            var result = await _routeService.GetRouteById(999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task When_AddRoute_Should_ReturnSuccess()
        {
            var bus = new Bus { BusId = 101, BusName = "TestBus", BusNumber = "TN01AB1234", BusType = "AC", TotalSeats = 10, Amenities = "Water", IsDeleted = false };
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            var dto = new RouteDTO
            {
                BusId = 101,
                Origin = "Tiruppur",
                Destination = "Salem",
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(4),
                Fare = 200,
                IsDeleted = false
            };

            var result = await _routeService.AddRoute(dto);

            Assert.That(result, Is.EqualTo("Route Added Successfully."));
        }

        [Test]
        public async Task When_UpdateRouteValid_Should_ReturnSuccess()
        {
            var route = new Route
            {
                RouteId = 20,
                BusId = 102,
                Origin = "Old",
                Destination = "Old",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(1),
                Fare = 150,
                IsDeleted = false
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            var updatedDto = new RouteDTO
            {
                RouteId = 20,
                BusId = 102,
                Origin = "NewOrigin",
                Destination = "NewDest",
                DepartureTime = DateTime.Now.AddHours(2),
                ArrivalTime = DateTime.Now.AddHours(5),
                Fare = 250,
                IsDeleted = false
            };

            var result = await _routeService.UpdateRoute(20, updatedDto);

            Assert.That(result, Is.EqualTo("Route Updated Successfully."));
        }

        [Test]
        public async Task When_DeleteRouteValid_Should_ReturnSuccess()
        {
            var route = new Route
            {
                RouteId = 30,
                BusId = 3,
                Origin = "Alpha",
                Destination = "Beta",
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(1),
                Fare = 180,
                IsDeleted = false
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            var result = await _routeService.DeleteRoute(30);

            Assert.That(result, Is.EqualTo("Route soft-deleted Successfully."));
        }

        [Test]
        public async Task When_SearchRoutes_Should_ReturnMatchingRoutes()
        {
            var fixedDate = new DateTime(2025, 07, 01); // Set only date, no time

            var bus = new Bus
            {
                BusId = 999,
                BusName = "TestBus",
                BusNumber = "TN99AA1234",
                BusType = "AC",
                TotalSeats = 10,
                Amenities = "Water",
                IsDeleted = false
            };

            await _context.Buses.AddAsync(bus);

            var route = new Route
            {
                RouteId = 505,
                BusId = 999,
                Origin = "Salem",
                Destination = "Erode",
                DepartureTime = fixedDate.AddHours(10), // 10 AM on 2025-07-01
                ArrivalTime = fixedDate.AddHours(14),   // 2 PM
                Fare = 200,
                IsDeleted = false
            };

            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();

            var result = await _routeService.SearchRoutes("Salem", "Erode", fixedDate);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

    }
}
