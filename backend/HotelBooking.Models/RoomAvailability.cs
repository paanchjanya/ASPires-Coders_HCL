using System;

namespace HotelBooking.Models
{
    public class RoomAvailability
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Available"; // Available, Booked, Maintenance, Unavailable
    }
}
