using AutoMapper;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Models.DTOs;
using FastX_BusTicketBooking.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using log4net;

namespace FastX_BusTicketBooking.API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public AuthService(AppDbContext context, IConfiguration config, IMapper mapper, ILog logger)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> Register(RegisterDTO registerDTO)
        {
            try
            {
                _logger.Info($"Register attempt for email: {registerDTO.Email}");

                if (await _context.Users.AnyAsync(u => u.Email == registerDTO.Email))
                {
                    _logger.Warn($"Registration failed: Email already exists - {registerDTO.Email}");
                    return "Email already exists.";
                }

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == registerDTO.RoleId);
                if (role == null)
                {
                    string? roleName = registerDTO.RoleId switch
                    {
                        1 => "Admin",
                        2 => "User",
                        3 => "BusOperator",
                        _ => null
                    };

                    if (roleName == null)
                    {
                        _logger.Warn("Invalid role ID during registration");
                        return "Invalid role selected.";
                    }

                    role = new Role
                    {
                        RoleId = registerDTO.RoleId,
                        RoleName = roleName
                    };

                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();
                    _logger.Info($"Role created: {role.RoleName}");
                }

                var user = _mapper.Map<User>(registerDTO);
                user.PasswordHash = HashPassword(registerDTO.Password);
                user.CreatedAt = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.Info($"User registered successfully: {user.Email}, UserId: {user.UserId}");
                return "User registered successfully.";
            }
            catch (Exception ex)
            {
                _logger.Error("Error during registration", ex);
                return $"Error: {ex.Message}";
            }
        }

        public async Task<object> Login(LoginDTO loginDTO)
        {
            try
            {
                _logger.Info($"Login attempt: {loginDTO.Email}");

                var user = await _context.Users.Include(u => u.Role)
                                               .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

                if (user == null || user.PasswordHash != HashPassword(loginDTO.Password))
                {
                    _logger.Warn($"Login failed for email: {loginDTO.Email}");
                    return new { Status = "Invalid Credentials" };
                }

                string token = GenerateToken(user);

                _logger.Info($"Login successful: {user.Email}, Token issued");

                return new
                {
                    Email = user.Email,
                    Token = token,
                    UserId = user.UserId,
                    Role = user.Role.RoleName
                };
            }
            catch (Exception ex)
            {
                _logger.Error("Error during login", ex);
                return new { Status = $"Error: {ex.Message}" };
            }
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:ValidIssuer"],
                audience: _config["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
