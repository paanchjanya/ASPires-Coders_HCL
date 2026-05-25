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
    public class LocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _context.Locations.ToListAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return NotFound(new { message = "Location not found." });
            return Ok(location);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] Location location)
        {
            var dbLocation = await _context.Locations.FindAsync(id);
            if (dbLocation == null) return NotFound(new { message = "Location not found." });

            dbLocation.Name = location.Name;
            await _context.SaveChangesAsync();
            return Ok(dbLocation);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var dbLocation = await _context.Locations.FindAsync(id);
            if (dbLocation == null) return NotFound(new { message = "Location not found." });

            _context.Locations.Remove(dbLocation);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Location deleted successfully." });
        }
    }
}
