using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HotelBooking.DAL;
using HotelBooking.Models;
using HotelBooking.Services.DTOs;

namespace HotelBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
            {
                return null;
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                RoleId = 3 // Default: User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dbUser = await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.Id == user.Id);

            var token = GenerateJwtToken(dbUser);

            return new AuthResponseDto
            {
                Token = token,
                Username = dbUser.Username,
                Email = dbUser.Email,
                Role = dbUser.Role?.Name ?? "User",
                AssignedHotelId = dbUser.AssignedHotelId
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name ?? "User",
                AssignedHotelId = user.AssignedHotelId
            };
        }

        public async Task<User?> GetProfileAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.AssignedHotel)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        private string GenerateJwtToken(User user)
        {
            var key = _config["Jwt:Key"] ?? "SUPER_SECRET_KEY_FOR_JWT_HOTEL_BOOKING_WEBSITE_2026";
            var issuer = _config["Jwt:Issuer"] ?? "HotelBookingAPI";
            var audience = _config["Jwt:Audience"] ?? "HotelBookingClient";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
