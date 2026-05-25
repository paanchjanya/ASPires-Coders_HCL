using System.Linq;
using System.Security.Claims;
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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-hotel")]
        public async Task<IActionResult> GetMyHotel()
        {
            var adminId = GetCurrentUserId();
            if (adminId == null) return Unauthorized(new { message = "User not identified." });

            var admin = await _context.Users.FindAsync(adminId.Value);
            if (admin == null || admin.AssignedHotelId == null)
            {
                return BadRequest(new { message = "Hotel Admin does not have an assigned hotel." });
            }

            var hotel = await _context.Hotels
                .Include(h => h.Location)
                .Include(h => h.HotelAmenities)
                .ThenInclude(ha => ha.Amenity)
                .FirstOrDefaultAsync(h => h.Id == admin.AssignedHotelId.Value);

            if (hotel == null) return NotFound(new { message = "Assigned hotel not found." });

            return Ok(hotel);
        }

        [HttpGet("my-hotel/bookings")]
        public async Task<IActionResult> GetMyHotelBookings()
        {
            var adminId = GetCurrentUserId();
            if (adminId == null) return Unauthorized(new { message = "User not identified." });

            var admin = await _context.Users.FindAsync(adminId.Value);
            if (admin == null || admin.AssignedHotelId == null)
            {
                return BadRequest(new { message = "Hotel Admin does not have an assigned hotel." });
            }

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b => b.Room!.HotelId == admin.AssignedHotelId.Value)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    Username = b.User!.Username,
                    b.RoomId,
                    RoomType = b.Room!.RoomType,
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

        [HttpGet("my-hotel/rooms")]
        public async Task<IActionResult> GetMyHotelRooms()
        {
            var adminId = GetCurrentUserId();
            if (adminId == null) return Unauthorized(new { message = "User not identified." });

            var admin = await _context.Users.FindAsync(adminId.Value);
            if (admin == null || admin.AssignedHotelId == null)
            {
                return BadRequest(new { message = "Hotel Admin does not have an assigned hotel." });
            }

            var rooms = await _context.Rooms
                .Where(r => r.HotelId == admin.AssignedHotelId.Value)
                .ToListAsync();

            return Ok(rooms);
        }

        private int? GetCurrentUserId()
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(nameIdentifier, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
