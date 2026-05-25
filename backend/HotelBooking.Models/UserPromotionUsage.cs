using System;

namespace HotelBooking.Models
{
    public class UserPromotionUsage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
