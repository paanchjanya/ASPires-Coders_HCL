using System;

namespace HotelBooking.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage"; // Flat, Percentage
        public decimal DiscountValue { get; set; }
        public bool Active { get; set; } = true;
        public DateTime ExpiryDate { get; set; }
    }
}
