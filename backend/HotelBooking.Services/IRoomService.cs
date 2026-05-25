using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
        Task<Room?> GetByIdAsync(int id);
        Task<Room> CreateAsync(Room room);
        Task<Room?> UpdateAsync(int id, Room room);
        Task<bool> UpdateRoomStatusAsync(int roomId, string status);
        Task<IEnumerable<RoomAvailability>> GetRoomAvailabilityScheduleAsync(int hotelId, DateTime checkIn, DateTime checkOut);
        Task<bool> UpdateRoomAvailabilityByDateAsync(int roomId, DateTime date, string status);
    }
}
