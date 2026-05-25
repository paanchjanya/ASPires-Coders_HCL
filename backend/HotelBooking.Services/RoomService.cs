using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBooking.DAL;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room> CreateAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room?> UpdateAsync(int id, Room room)
        {
            var dbRoom = await _context.Rooms.FindAsync(id);
            if (dbRoom == null) return null;

            dbRoom.RoomType = room.RoomType;
            dbRoom.PricePerNight = room.PricePerNight;
            dbRoom.Capacity = room.Capacity;
            dbRoom.Description = room.Description;
            dbRoom.ImageUrl = room.ImageUrl;
            dbRoom.Status = room.Status;

            await _context.SaveChangesAsync();
            return dbRoom;
        }

        public async Task<bool> UpdateRoomStatusAsync(int roomId, string status)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return false;

            room.Status = status; // Available, Booked, Maintenance, Unavailable
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomAvailability>> GetRoomAvailabilityScheduleAsync(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            var rooms = await _context.Rooms.Where(r => r.HotelId == hotelId).Select(r => r.Id).ToListAsync();

            return await _context.RoomAvailabilities
                .Include(ra => ra.Room)
                .Where(ra => rooms.Contains(ra.RoomId) && ra.Date >= checkIn.Date && ra.Date <= checkOut.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateRoomAvailabilityByDateAsync(int roomId, DateTime date, string status)
        {
            var availability = await _context.RoomAvailabilities
                .FirstOrDefaultAsync(ra => ra.RoomId == roomId && ra.Date.Date == date.Date);

            if (availability == null)
            {
                availability = new RoomAvailability
                {
                    RoomId = roomId,
                    Date = date.Date,
                    Status = status
                };
                _context.RoomAvailabilities.Add(availability);
            }
            else
            {
                availability.Status = status;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
