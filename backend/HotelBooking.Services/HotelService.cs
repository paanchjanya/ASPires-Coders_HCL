using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBooking.DAL;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class HotelService : IHotelService
    {
        private readonly AppDbContext _context;

        public HotelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            return await _context.Hotels
                .Include(h => h.Location)
                .Include(h => h.Rooms)
                .Include(h => h.HotelAmenities)
                .ThenInclude(ha => ha.Amenity)
                .ToListAsync();
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.Location)
                .Include(h => h.Rooms)
                .Include(h => h.HotelAmenities)
                .ThenInclude(ha => ha.Amenity)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<Hotel>> GetByLocationIdAsync(int locationId)
        {
            return await _context.Hotels
                .Include(h => h.Location)
                .Include(h => h.Rooms)
                .Include(h => h.HotelAmenities)
                .ThenInclude(ha => ha.Amenity)
                .Where(h => h.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string location, DateTime checkIn, DateTime checkOut, int guests, int rooms, decimal? minPrice, decimal? maxPrice, string? category)
        {
            var query = _context.Hotels
                .Include(h => h.Location)
                .Include(h => h.Rooms)
                .Include(h => h.HotelAmenities)
                .ThenInclude(ha => ha.Amenity)
                .AsQueryable();

            // 1. Filter by Location Name (Case Insensitive)
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(h => h.Location!.Name.ToLower().Contains(location.ToLower()));
            }

            var hotels = await query.ToListAsync();
            var filteredHotels = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                var matchingRooms = new List<Room>();

                foreach (var room in hotel.Rooms)
                {
                    // Filter room status
                    if (room.Status == "Maintenance" || room.Status == "Unavailable")
                    {
                        continue;
                    }

                    // Filter by room capacity
                    if (room.Capacity < guests)
                    {
                        continue;
                    }

                    // Filter by Price Range
                    if (minPrice.HasValue && room.PricePerNight < minPrice.Value)
                    {
                        continue;
                    }
                    if (maxPrice.HasValue && room.PricePerNight > maxPrice.Value)
                    {
                        continue;
                    }

                    // Filter by category
                    if (!string.IsNullOrEmpty(category) && !room.RoomType.ToLower().Contains(category.ToLower()))
                    {
                        continue;
                    }

                    // Check date-specific availability inside RoomAvailability
                    var isRoomBlocked = await _context.RoomAvailabilities
                        .AnyAsync(ra => ra.RoomId == room.Id &&
                                        ra.Date >= checkIn.Date &&
                                        ra.Date < checkOut.Date &&
                                        (ra.Status == "Maintenance" || ra.Status == "Unavailable" || ra.Status == "Booked"));

                    if (isRoomBlocked)
                    {
                        continue;
                    }

                    // Verify active booking date overlap
                    var overlappingBookings = await _context.Bookings
                        .Where(b => b.RoomId == room.Id &&
                                    b.Status == "Confirmed" &&
                                    b.CheckInDate < checkOut.Date &&
                                    checkIn.Date < b.CheckOutDate)
                        .CountAsync();

                    if (overlappingBookings >= rooms) // if booked rooms count matches/exceeds quantity requested
                    {
                        continue;
                    }

                    matchingRooms.Add(room);
                }

                // If hotel has any room matching all search criteria, include it
                if (matchingRooms.Count > 0)
                {
                    hotel.Rooms = matchingRooms;
                    filteredHotels.Add(hotel);
                }
            }

            return filteredHotels;
        }

        public async Task<Hotel> CreateAsync(Hotel hotel, List<int> amenityIds)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            if (amenityIds != null && amenityIds.Count > 0)
            {
                foreach (var amenityId in amenityIds)
                {
                    _context.HotelAmenities.Add(new HotelAmenity { HotelId = hotel.Id, AmenityId = amenityId });
                }
                await _context.SaveChangesAsync();
            }

            return hotel;
        }

        public async Task<Hotel?> UpdateAsync(int id, Hotel hotel, List<int> amenityIds)
        {
            var dbHotel = await _context.Hotels
                .Include(h => h.HotelAmenities)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (dbHotel == null) return null;

            dbHotel.Name = hotel.Name;
            dbHotel.Description = hotel.Description;
            dbHotel.ImageUrl = hotel.ImageUrl;
            dbHotel.Rating = hotel.Rating;
            if (hotel.LocationId > 0)
            {
                dbHotel.LocationId = hotel.LocationId;
            }

            // Sync Amenities
            _context.HotelAmenities.RemoveRange(dbHotel.HotelAmenities);
            if (amenityIds != null && amenityIds.Count > 0)
            {
                foreach (var amenityId in amenityIds)
                {
                    _context.HotelAmenities.Add(new HotelAmenity { HotelId = dbHotel.Id, AmenityId = amenityId });
                }
            }

            await _context.SaveChangesAsync();
            return dbHotel;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbHotel = await _context.Hotels.FindAsync(id);
            if (dbHotel == null) return false;

            _context.Hotels.Remove(dbHotel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
