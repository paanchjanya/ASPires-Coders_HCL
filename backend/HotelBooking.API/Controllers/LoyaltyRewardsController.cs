using System;
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
    [Route("api/rewards")]
    [Authorize]
    public class LoyaltyRewardsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoyaltyRewardsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-points")]
        public async Task<IActionResult> GetMyPoints()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User not identified." });

            var reward = await _context.LoyaltyRewards
                .FirstOrDefaultAsync(lr => lr.UserId == userId.Value);

            if (reward == null)
            {
                // Create a default reward record if none exists
                reward = new LoyaltyReward
                {
                    UserId = userId.Value,
                    TotalPoints = 0,
                    RedeemedPoints = 0,
                    AvailablePoints = 0,
                    LastUpdated = DateTime.UtcNow
                };
                _context.LoyaltyRewards.Add(reward);
                await _context.SaveChangesAsync();
            }

            string badge = "Bronze";
            if (reward.TotalPoints > 3000)
            {
                badge = "Gold";
            }
            else if (reward.TotalPoints >= 1000)
            {
                badge = "Silver";
            }

            return Ok(new
            {
                reward.TotalPoints,
                reward.AvailablePoints,
                reward.RedeemedPoints,
                reward.LastUpdated,
                Badge = badge
            });
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
