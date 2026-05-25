using System;

namespace HotelBooking.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty; // Card, PayPal, etc.
        public string Status { get; set; } = "Success"; // Success, Failed
    }
}
