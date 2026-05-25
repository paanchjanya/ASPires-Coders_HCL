using System.Text.Json.Serialization;

namespace HotelBooking.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int? AssignedHotelId { get; set; } // For Hotel Admins
        public Hotel? AssignedHotel { get; set; }
    }
}
