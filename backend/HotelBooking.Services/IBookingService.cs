using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Services.DTOs;

namespace HotelBooking.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto?> BookRoomAsync(BookingRequestDto request, int userId);
        Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int userId);
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<bool> CancelBookingAsync(int bookingId, int userId, bool isAdmin);
        Task<bool> CheckAvailabilityAsync(int roomId, System.DateTime checkIn, System.DateTime checkOut, int roomsCount = 1);
        Task<HotelBooking.Models.EmailLog?> GetEmailLogByBookingIdAsync(int bookingId, int userId, bool isAdmin);
    }
}
