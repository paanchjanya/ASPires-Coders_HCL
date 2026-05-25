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
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hotels = await _hotelService.GetAllAsync();
            return Ok(hotels);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? location, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int guests, [FromQuery] int rooms = 1, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null, [FromQuery] string? category = null)
        {
            // Default dates if invalid
            if (checkIn == default) checkIn = DateTime.Today;
            if (checkOut == default) checkOut = DateTime.Today.AddDays(1);
            if (guests <= 0) guests = 1;

            var results = await _hotelService.SearchHotelsAsync(location ?? "", checkIn, checkOut, guests, rooms, minPrice, maxPrice, category);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound(new { message = "Hotel not found." });
            return Ok(hotel);
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetByLocationId(int locationId)
        {
            var hotels = await _hotelService.GetByLocationIdAsync(locationId);
            return Ok(hotels);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] HotelRequestDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                LocationId = dto.LocationId,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Rating = dto.Rating
            };
            var result = await _hotelService.CreateAsync(hotel, dto.AmenityIds);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] HotelRequestDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                LocationId = dto.LocationId,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Rating = dto.Rating
            };
            var result = await _hotelService.UpdateAsync(id, hotel, dto.AmenityIds);
            if (result == null) return NotFound(new { message = "Hotel not found." });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _hotelService.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Hotel not found." });
            return Ok(new { message = "Hotel deleted successfully." });
        }
    }
}
