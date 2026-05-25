using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class SeasonalOffer
    {
        [Key]
        public int OfferId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage"; // Flat, Percentage
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ApplicableLocation { get; set; } // e.g. Whitefield
        public int? ApplicableHotel { get; set; } // HotelId mapping
        public bool IsActive { get; set; } = true;
    }
}
