using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Models;
using HotelBooking.Services;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _promotionService.GetAllAsync();
            return Ok(promotions);
        }

        [HttpGet("validate/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateCode(string code)
        {
            var promotion = await _promotionService.GetByCodeAsync(code);
            if (promotion == null)
            {
                return NotFound(new { message = "Invalid or expired promotion code." });
            }
            return Ok(new
            {
                promotion.Code,
                promotion.DiscountType,
                promotion.DiscountValue,
                promotion.ExpiryDate
            });
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] Promotion promotion)
        {
            var result = await _promotionService.CreateAsync(promotion);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] Promotion promotion)
        {
            var result = await _promotionService.UpdateAsync(id, promotion);
            if (result == null) return NotFound(new { message = "Promotion not found." });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Promotion not found." });
            return Ok(new { message = "Promotion deleted successfully." });
        }
    }
}
