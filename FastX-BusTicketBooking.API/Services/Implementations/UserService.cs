using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastX_BusTicketBooking.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public UserService(AppDbContext context, IMapper mapper, ILog logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            try
            {
                _logger.Info("Fetching all users...");

                var users = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .Include(u => u.Role)
                    .ToListAsync();

                _logger.Info($"Total users fetched: {users.Count}");
                return _mapper.Map<IEnumerable<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching users", ex);
                throw new Exception($"Error fetching users: {ex.Message}");
            }
        }

        public async Task<UserDTO?> GetUserById(int id)
        {
            try
            {
                _logger.Info($"Fetching user by ID: {id}");

                var user = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    _logger.Warn($"User not found: UserId={id}");
                    return null;
                }

                _logger.Info($"User found: UserId={id}");
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching user by ID: {id}", ex);
                throw new Exception($"Error fetching user by ID: {ex.Message}");
            }
        }

        public async Task<string> DeleteUser(int id)
        {
            try
            {
                _logger.Info($"Attempting to delete user: UserId={id}");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);

                if (user == null)
                {
                    _logger.Warn($"User not found to delete: UserId={id}");
                    return "User not found.";
                }

                user.IsDeleted = true;
                await _context.SaveChangesAsync();

                _logger.Info($"User deleted successfully: UserId={id}");
                return "User soft-deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user: UserId={id}", ex);
                return $"Error deleting user: {ex.Message}";
            }
        }
    }
}
