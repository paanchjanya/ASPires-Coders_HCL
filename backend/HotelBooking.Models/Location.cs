using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelBooking.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
