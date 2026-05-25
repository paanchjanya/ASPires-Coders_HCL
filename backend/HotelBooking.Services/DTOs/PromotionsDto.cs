using System;

namespace HotelBooking.Services.DTOs
{
    public class PromoApplyDto
    {
        public string Code { get; set; } = string.Empty;
    }

    public class RedeemPointsDto
    {
        public int PointsToRedeem { get; set; }
    }

    public class PromotionResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage"; // Flat, Percentage
        public decimal DiscountValue { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class LoyaltyPointsResponseDto
    {
        public int AvailablePoints { get; set; }
        public int TotalPoints { get; set; }
        public int RedeemedPoints { get; set; }
        public string MemberTier { get; set; } = "Bronze"; // Bronze, Silver, Gold
    }

    public class LoyaltyHistoryDto
    {
        public string TransactionType { get; set; } = "Earned"; // Earned, Redeemed
        public int Points { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
