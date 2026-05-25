using System.Text.Json.Serialization;

namespace HotelBooking.Models
{
    public class HotelAmenity
    {
        public int HotelId { get; set; }

        [JsonIgnore]
        public Hotel? Hotel { get; set; }
        public int AmenityId { get; set; }
        public Amenity? Amenity { get; set; }
    }
}
