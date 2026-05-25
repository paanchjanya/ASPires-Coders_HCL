using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBooking.DAL;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly AppDbContext _context;

        public PromotionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _context.Promotions.ToListAsync();
        }

        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code.ToUpper() == code.ToUpper() && p.Active);
        }

        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<Promotion?> UpdateAsync(int id, Promotion promotion)
        {
            var dbPromo = await _context.Promotions.FindAsync(id);
            if (dbPromo == null) return null;

            dbPromo.Code = promotion.Code;
            dbPromo.DiscountType = promotion.DiscountType;
            dbPromo.DiscountValue = promotion.DiscountValue;
            dbPromo.Active = promotion.Active;
            dbPromo.ExpiryDate = promotion.ExpiryDate;

            await _context.SaveChangesAsync();
            return dbPromo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbPromo = await _context.Promotions.FindAsync(id);
            if (dbPromo == null) return false;

            _context.Promotions.Remove(dbPromo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
