using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion?> GetByCodeAsync(string code);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion?> UpdateAsync(int id, Promotion promotion);
        Task<bool> DeleteAsync(int id);
    }
}
