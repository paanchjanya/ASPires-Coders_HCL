using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<Hotel?> GetByIdAsync(int id);
        Task<IEnumerable<Hotel>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string location, DateTime checkIn, DateTime checkOut, int guests, int rooms, decimal? minPrice, decimal? maxPrice, string? category);
        Task<Hotel> CreateAsync(Hotel hotel, List<int> amenityIds);
        Task<Hotel?> UpdateAsync(int id, Hotel hotel, List<int> amenityIds);
        Task<bool> DeleteAsync(int id);
    }
}
