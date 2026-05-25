using System.Text.Json.Serialization;

namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }

        [JsonIgnore]
        public Hotel? Hotel { get; set; }
        public string RoomType { get; set; } = string.Empty; // Standard, Deluxe, Suite
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; // Available, Booked, Maintenance, Unavailable
    }
}
