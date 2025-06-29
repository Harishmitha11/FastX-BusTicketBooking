using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.Entities;
using FastX_BusTicketBooking.API.Services.Implementations;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using AutoMapper;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastX_BusTicketBooking.API.Models.DTOs;

namespace FastX_BusTicketBooking.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private AppDbContext _context;
        private IUserService _userService;
        private IMapper _mapper;
        private Mock<ILog> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UserTestDb")
                .Options;

            _context = new AppDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            });

            _mapper = config.CreateMapper();
            _mockLogger = new Mock<ILog>();

            
            var role = new Role { RoleId = 1, RoleName = "Admin" };
            _context.Roles.Add(role);
            _context.Users.AddRange(
                new User
                {
                    UserId = 1,
                    Name = "Alice",
                    Email = "alice@example.com",
                    Gender = "Female",
                    ContactNumber = "9876543210",
                    Address = "Wonderland",
                    RoleId = 1,
                    Role = role,
                    IsDeleted = false,
                    PasswordHash = "dummyHash1"
                },
                new User
                {
                    UserId = 2,
                    Name = "Bob",
                    Email = "bob@example.com",
                    Gender = "Male",
                    ContactNumber = "9999999999",
                    Address = "Builderland",
                    RoleId = 1,
                    Role = role,
                    IsDeleted = false,
                    PasswordHash = "dummyHash2"
                }
            );
            _context.SaveChanges();

            _userService = new UserService(_context, _mapper, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task When_GetAllUsers_Should_ReturnList()
        {
            var result = await _userService.GetAllUsers();
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task When_GetUserByIdExists_Should_ReturnUser()
        {
            var result = await _userService.GetUserById(1);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Alice"));
        }

        [Test]
        public async Task When_GetUserByIdInvalid_Should_ReturnNull()
        {
            var result = await _userService.GetUserById(999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task When_DeleteUserValid_Should_MarkDeleted()
        {
            var result = await _userService.DeleteUser(1);
            var user = await _context.Users.FindAsync(1);

            Assert.That(result, Is.EqualTo("User soft-deleted successfully."));
            Assert.That(user!.IsDeleted, Is.True);
        }

        [Test]
        public async Task When_DeleteUserInvalid_Should_ReturnNotFound()
        {
            var newUser = new User
            {
                UserId = 99,
                Name = "Temp",
                Email = "temp@example.com",
                Gender = "Other",
                ContactNumber = "9000000000",
                Address = "Nowhere",
                RoleId = 1,
                Role = _context.Roles.First(),
                IsDeleted = true,
                PasswordHash = "dummy"
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            var result = await _userService.DeleteUser(99);
            Assert.That(result, Is.EqualTo("User not found."));
        }
    }
}
