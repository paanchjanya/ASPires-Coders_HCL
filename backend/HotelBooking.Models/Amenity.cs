using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelBooking.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
    }
}
