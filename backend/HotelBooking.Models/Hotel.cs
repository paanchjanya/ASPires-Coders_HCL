using System.Collections.Generic;

namespace HotelBooking.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public Location? Location { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
    }
}
