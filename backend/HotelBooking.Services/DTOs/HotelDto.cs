using System.Collections.Generic;

namespace HotelBooking.Services.DTOs
{
    public class HotelRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public List<int> AmenityIds { get; set; } = new List<int>();
    }
}
