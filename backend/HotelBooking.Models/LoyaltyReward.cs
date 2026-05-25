using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class LoyaltyReward
    {
        [Key]
        public int RewardId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int TotalPoints { get; set; }
        public int RedeemedPoints { get; set; }
        public int AvailablePoints { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
