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
    [Route("api/offers")]
    public class SeasonalOffersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeasonalOffersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOffers()
        {
            var today = DateTime.Today;
            var offers = await _context.SeasonalOffers
                .Where(o => o.IsActive && today >= o.StartDate && today <= o.EndDate)
                .ToListAsync();
            return Ok(offers);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetAllOffers()
        {
            var offers = await _context.SeasonalOffers.ToListAsync();
            return Ok(offers);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] SeasonalOffer offer)
        {
            _context.SeasonalOffers.Add(offer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetActiveOffers), new { id = offer.OfferId }, offer);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] SeasonalOffer offer)
        {
            var dbOffer = await _context.SeasonalOffers.FindAsync(id);
            if (dbOffer == null) return NotFound(new { message = "Offer not found." });

            dbOffer.Title = offer.Title;
            dbOffer.Description = offer.Description;
            dbOffer.DiscountType = offer.DiscountType;
            dbOffer.DiscountValue = offer.DiscountValue;
            dbOffer.StartDate = offer.StartDate;
            dbOffer.EndDate = offer.EndDate;
            dbOffer.ApplicableLocation = offer.ApplicableLocation;
            dbOffer.ApplicableHotel = offer.ApplicableHotel;
            dbOffer.IsActive = offer.IsActive;

            await _context.SaveChangesAsync();
            return Ok(dbOffer);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var dbOffer = await _context.SeasonalOffers.FindAsync(id);
            if (dbOffer == null) return NotFound(new { message = "Offer not found." });

            _context.SeasonalOffers.Remove(dbOffer);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Offer deleted successfully." });
        }
    }
}
