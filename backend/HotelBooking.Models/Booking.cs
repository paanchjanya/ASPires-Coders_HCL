using System;

namespace HotelBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int PointsRedeemed { get; set; } = 0;
        public string ReservationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled
        public int RoomsCount { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
