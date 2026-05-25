using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.DAL;
using HotelBooking.Models;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuperAdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.RoleId == 3);
            var totalAdmins = await _context.Users.CountAsync(u => u.RoleId == 2);
            var totalHotels = await _context.Hotels.CountAsync();
            var totalLocations = await _context.Locations.CountAsync();
            var bookings = await _context.Bookings.Where(b => b.Status == "Confirmed").ToListAsync();
            var totalRevenue = bookings.Sum(b => b.TotalPrice);
            var totalBookingsCount = bookings.Count;

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalAdmins = totalAdmins,
                TotalHotels = totalHotels,
                TotalLocations = totalLocations,
                TotalRevenue = totalRevenue,
                TotalBookingsCount = totalBookingsCount
            });
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateHotelAdmin([FromBody] CreateAdminDto dto)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
            {
                return BadRequest(new { message = "Email is already registered." });
            }

            var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == dto.AssignedHotelId);
            if (!hotelExists)
            {
                return BadRequest(new { message = "Assigned hotel does not exist." });
            }

            var admin = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                RoleId = 2, // Admin
                AssignedHotelId = dto.AssignedHotelId
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = admin.Id,
                admin.Username,
                admin.Email,
                admin.RoleId,
                admin.AssignedHotelId
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.AssignedHotel)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    RoleName = u.Role!.Name,
                    HotelName = u.AssignedHotel != null ? u.AssignedHotel.Name : null,
                    u.AssignedHotelId
                })
                .ToListAsync();
            return Ok(users);
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r!.Hotel)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    Username = b.User!.Username,
                    b.RoomId,
                    RoomType = b.Room!.RoomType,
                    HotelName = b.Room.Hotel!.Name,
                    b.CheckInDate,
                    b.CheckOutDate,
                    b.TotalPrice,
                    b.ReservationNumber,
                    b.Status,
                    b.CreatedAt
                })
                .ToListAsync();
            return Ok(bookings);
        }

        [HttpGet("hotels")]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Location)
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    LocationName = h.Location!.Name,
                    h.Rating
                })
                .ToListAsync();
            return Ok(hotels);
        }
    }

    public class CreateAdminDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int AssignedHotelId { get; set; }
    }
}
