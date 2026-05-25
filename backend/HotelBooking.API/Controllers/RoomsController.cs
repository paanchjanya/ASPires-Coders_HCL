using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Models;
using HotelBooking.Services;
using HotelBooking.Services.DTOs;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetByHotelId(int hotelId)
        {
            var rooms = await _roomService.GetByHotelIdAsync(hotelId);
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound(new { message = "Room not found." });
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create([FromBody] RoomRequestDto dto)
        {
            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomType = dto.RoomType,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Status = "Available"
            };

            var result = await _roomService.CreateAsync(room);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomRequestDto dto)
        {
            var room = new Room
            {
                RoomType = dto.RoomType,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl
            };

            var result = await _roomService.UpdateAsync(id, room);
            if (result == null) return NotFound(new { message = "Room not found." });
            return Ok(result);
        }

        [HttpPut("status/{roomId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateStatus(int roomId, [FromBody] string status)
        {
            var success = await _roomService.UpdateRoomStatusAsync(roomId, status);
            if (!success) return NotFound(new { message = "Room not found." });
            return Ok(new { message = "Room status updated successfully." });
        }

        [HttpGet("availability")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailabilitySchedule([FromQuery] int hotelId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var schedule = await _roomService.GetRoomAvailabilityScheduleAsync(hotelId, checkIn, checkOut);
            return Ok(schedule);
        }

        [HttpPut("{roomId}/availability-by-date")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateAvailabilityByDate(int roomId, [FromQuery] DateTime date, [FromQuery] string status)
        {
            var success = await _roomService.UpdateRoomAvailabilityByDateAsync(roomId, date, status);
            if (!success) return NotFound(new { message = "Failed to update room availability." });
            return Ok(new { message = "Room availability updated successfully." });
        }
    }
}
