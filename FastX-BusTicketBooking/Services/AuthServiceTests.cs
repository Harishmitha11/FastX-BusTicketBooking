using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastX_BusTicketBooking.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AppDbContext _context;
        private IConfiguration _config;
        private IMapper _mapper;
        private Mock<ILog> _mockLogger;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("AuthDbTest")
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterDTO, User>();
            });

            _mapper = config.CreateMapper();
            _mockLogger = new Mock<ILog>();

            var configData = new Dictionary<string, string>
            {
                { "Jwt:Secret", "ThisIsASecretKeyForJwtTokenAuthTest" },
                { "Jwt:ValidIssuer", "TestIssuer" },
                { "Jwt:ValidAudience", "TestAudience" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            _authService = new AuthService(_context, _config, _mapper, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
        [Test]
        public async Task When_RegisterValid_Should_CreateUser()
        {
            var dto = new RegisterDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Test123",
                Gender = "Female",
                ContactNumber = "9876543210",
                Address = "Test Address",
                RoleId = 2
            };

            var result = await _authService.Register(dto);
            Assert.That(result, Is.EqualTo("User registered successfully."));
        }

        [Test]
        public async Task When_RegisterDuplicateEmail_Should_ReturnError()
        {
            var dto = new RegisterDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Test123",
                Gender = "Female",
                ContactNumber = "9876543210",
                Address = "Test Address",
                RoleId = 2
            };

            await _authService.Register(dto);

            var duplicateResult = await _authService.Register(dto);
            Assert.That(duplicateResult, Is.EqualTo("Email already exists."));
        }

        [Test]
        public async Task When_LoginValid_Should_ReturnTokenObject()
        {
            var registerDTO = new RegisterDTO
            {
                Name = "Login User",
                Email = "login@example.com",
                Password = "Login123",
                Gender = "Male",
                ContactNumber = "8888888888",
                Address = "Login Address",
                RoleId = 2
            };

            await _authService.Register(registerDTO);

            var loginDTO = new LoginDTO
            {
                Email = "login@example.com",
                Password = "Login123"
            };

            var result = await _authService.Login(loginDTO);
            var token = result?.GetType().GetProperty("Token")?.GetValue(result, null)?.ToString();

            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task When_LoginInvalid_Should_ReturnInvalidCredentials()
        {
            var loginDTO = new LoginDTO
            {
                Email = "notexist@example.com",
                Password = "WrongPass"
            };

            var result = await _authService.Login(loginDTO);
            var status = result?.GetType().GetProperty("Status")?.GetValue(result, null)?.ToString();

            Assert.That(status, Is.EqualTo("Invalid Credentials"));
        }
    }
}
