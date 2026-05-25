using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Services;
using HotelBooking.Services.DTOs;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> BookRoom([FromBody] BookingRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            var result = await _bookingService.BookRoomAsync(request, userId.Value);
            if (result == null)
            {
                return BadRequest(new { message = "The selected room is not available for the specified dates, or the booking details are invalid." });
            }

            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            var bookings = await _bookingService.GetMyBookingsAsync(userId.Value);
            return Ok(bookings);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            var isAdmin = User.IsInRole("Admin");

            var success = await _bookingService.CancelBookingAsync(id, userId.Value, isAdmin);
            if (!success)
            {
                return BadRequest(new { message = "Booking cannot be cancelled. It may already be cancelled, or you do not have permission." });
            }

            return Ok(new { message = "Booking cancelled successfully." });
        }

        [HttpGet("check-availability")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAvailability([FromQuery] int roomId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int roomsCount = 1)
        {
            var isAvailable = await _bookingService.CheckAvailabilityAsync(roomId, checkIn, checkOut, roomsCount);
            return Ok(new { roomId, checkIn, checkOut, roomsCount, isAvailable });
        }

        [HttpGet("{id}/email-log")]
        public async Task<IActionResult> GetBookingEmailLog(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var emailLog = await _bookingService.GetEmailLogByBookingIdAsync(id, userId.Value, isAdmin);

            if (emailLog == null)
            {
                return NotFound(new { message = "Email log not found, or you do not have permission to view it." });
            }

            return Ok(emailLog);
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
