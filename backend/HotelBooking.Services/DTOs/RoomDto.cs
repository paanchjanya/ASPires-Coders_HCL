using System.Collections.Generic;

namespace HotelBooking.Services.DTOs
{
    public class RoomRequestDto
    {
        public int HotelId { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public int TotalRoomsCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<int> AmenityIds { get; set; } = new List<int>();
    }
}
