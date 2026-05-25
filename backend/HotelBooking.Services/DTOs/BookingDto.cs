using System;

namespace HotelBooking.Services.DTOs
{
    public class BookingRequestDto
    {
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string PromoCode { get; set; } = string.Empty;
        public int RedeemPoints { get; set; } = 0;
        public string PaymentMethod { get; set; } = "Card";
        public int Guests { get; set; } = 1;
        public int RoomsCount { get; set; } = 1;
    }

    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string HotelLocation { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RoomsCount { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
    }
}
