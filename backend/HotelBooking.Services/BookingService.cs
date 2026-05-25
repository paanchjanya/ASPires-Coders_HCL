using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBooking.DAL;
using HotelBooking.Models;
using HotelBooking.Services.DTOs;

namespace HotelBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut, int roomsCount = 1)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || room.Status == "Maintenance" || room.Status == "Unavailable")
            {
                return false;
            }

            var checkInDate = checkIn.Date;
            var checkOutDate = checkOut.Date;

            if (checkInDate >= checkOutDate || checkInDate < DateTime.Today)
            {
                return false;
            }

            // 1. Check date-specific blocks in RoomAvailability
            var isBlocked = await _context.RoomAvailabilities
                .AnyAsync(ra => ra.RoomId == roomId &&
                                ra.Date >= checkInDate &&
                                ra.Date < checkOutDate &&
                                (ra.Status == "Maintenance" || ra.Status == "Unavailable" || ra.Status == "Booked"));

            if (isBlocked) return false;

            // 2. Check overlap with active bookings
            var overlappingBookingsCount = await _context.Bookings
                .Where(b => b.RoomId == roomId && b.Status == "Confirmed" &&
                            b.CheckInDate < checkOutDate && checkInDate < b.CheckOutDate)
                .SumAsync(b => b.RoomsCount);

            // Seeded room capacity is 10, check if booking roomsCount exceeds remaining
            return (overlappingBookingsCount + roomsCount) <= 10;
        }

        public async Task<BookingResponseDto?> BookRoomAsync(BookingRequestDto request, int userId)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .ThenInclude(h => h!.Location)
                .FirstOrDefaultAsync(r => r.Id == request.RoomId);

            if (room == null) return null;

            if (request.RoomsCount < 1 || request.RoomsCount > 10)
            {
                throw new InvalidOperationException("You can only book between 1 and 10 rooms at a time.");
            }

            var isAvailable = await CheckAvailabilityAsync(request.RoomId, request.CheckInDate, request.CheckOutDate, request.RoomsCount);
            if (!isAvailable) return null;

            var nights = (request.CheckOutDate.Date - request.CheckInDate.Date).Days;
            if (nights <= 0) return null;

            var basePrice = room.PricePerNight * nights * request.RoomsCount;
            var finalPrice = basePrice;

            // 1. AUTO-APPLY SEASONAL OFFERS (highest discount gets applied)
            var activeOffers = await _context.SeasonalOffers
                .Where(s => s.IsActive && request.CheckInDate.Date >= s.StartDate.Date && request.CheckInDate.Date <= s.EndDate.Date)
                .ToListAsync();

            SeasonalOffer? appliedOffer = null;
            decimal seasonalDiscount = 0;

            foreach (var offer in activeOffers)
            {
                // Check location matching
                if (!string.IsNullOrEmpty(offer.ApplicableLocation) &&
                    room.Hotel?.Location?.Name.ToLower() != offer.ApplicableLocation.ToLower())
                {
                    continue;
                }

                // Check hotel matching
                if (offer.ApplicableHotel.HasValue && room.HotelId != offer.ApplicableHotel.Value)
                {
                    continue;
                }

                decimal discountVal = 0;
                if (offer.DiscountType == "Percentage")
                {
                    discountVal = basePrice * (offer.DiscountValue / 100);
                }
                else // Flat
                {
                    discountVal = offer.DiscountValue;
                }

                if (discountVal > seasonalDiscount)
                {
                    seasonalDiscount = discountVal;
                    appliedOffer = offer;
                }
            }

            finalPrice -= seasonalDiscount;

            // 2. APPLY DISCOUNT COUPONS (if provided)
            Promotion? promotion = null;
            decimal couponDiscount = 0;

            if (!string.IsNullOrEmpty(request.PromoCode))
            {
                promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Code.ToUpper() == request.PromoCode.ToUpper() && p.Active && p.ExpiryDate >= DateTime.Today);

                if (promotion == null)
                {
                    throw new InvalidOperationException("Invalid or expired coupon code.");
                }

                // First-time coupon checks
                if (promotion.Code.ToUpper() == "FIRST500")
                {
                    var priorBookings = await _context.Bookings
                        .AnyAsync(b => b.UserId == userId && b.Status == "Confirmed");
                    if (priorBookings)
                    {
                        throw new InvalidOperationException("Coupon 'FIRST500' is only applicable to first-time bookings.");
                    }
                }

                // One-time usage validation
                var hasUsed = await _context.UserPromotionUsages
                    .AnyAsync(upu => upu.UserId == userId && upu.PromotionId == promotion.Id);
                if (hasUsed)
                {
                    throw new InvalidOperationException($"Coupon code '{promotion.Code.ToUpper()}' has already been redeemed by you.");
                }

                // Calculate coupon discount
                if (promotion.DiscountType == "Percentage")
                {
                    couponDiscount = finalPrice * (promotion.DiscountValue / 100);
                }
                else // Flat
                {
                    couponDiscount = promotion.DiscountValue;
                }

                finalPrice -= couponDiscount;
            }

            // 3. REDEEM LOYALTY REWARDS (₹1 per point discount)
            decimal loyaltyDiscount = 0;
            LoyaltyReward? loyaltyRecord = null;

            if (request.RedeemPoints > 0)
            {
                loyaltyRecord = await _context.LoyaltyRewards
                    .FirstOrDefaultAsync(lr => lr.UserId == userId);

                if (loyaltyRecord == null || loyaltyRecord.AvailablePoints < request.RedeemPoints)
                {
                    throw new InvalidOperationException("Insufficient loyalty points balance.");
                }

                loyaltyDiscount = request.RedeemPoints;
                finalPrice -= loyaltyDiscount;

                // Update points deduction
                loyaltyRecord.RedeemedPoints += request.RedeemPoints;
                loyaltyRecord.AvailablePoints -= request.RedeemPoints;
                loyaltyRecord.LastUpdated = DateTime.UtcNow;
            }

            // Final Price must not be negative
            finalPrice = Math.Max(0, finalPrice);

            // Generate Booking
            var reservationNumber = "BLR" + GenerateReservationNumber();
            var booking = new Booking
            {
                UserId = userId,
                RoomId = request.RoomId,
                CheckInDate = request.CheckInDate.Date,
                CheckOutDate = request.CheckOutDate.Date,
                TotalPrice = finalPrice,
                ReservationNumber = reservationNumber,
                Status = "Confirmed",
                RoomsCount = request.RoomsCount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Track coupon usage
            if (promotion != null)
            {
                _context.UserPromotionUsages.Add(new UserPromotionUsage
                {
                    UserId = userId,
                    PromotionId = promotion.Id,
                    UsedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            // 4. ADD LOYALTY REWARDS POINTS FOR THE CURRENT BOOKING (100 points per ₹1000 spent)
            int earnedPoints = (int)(finalPrice / 1000) * 100;
            if (earnedPoints > 0)
            {
                if (loyaltyRecord == null)
                {
                    loyaltyRecord = new LoyaltyReward
                    {
                        UserId = userId,
                        TotalPoints = earnedPoints,
                        RedeemedPoints = 0,
                        AvailablePoints = earnedPoints,
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.LoyaltyRewards.Add(loyaltyRecord);
                }
                else
                {
                    loyaltyRecord.TotalPoints += earnedPoints;
                    loyaltyRecord.AvailablePoints += earnedPoints;
                    loyaltyRecord.LastUpdated = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }

            // 5. TRIGGER SIMULATED BOOKING EMAIL CONFIRMATION LOG
            var user = await _context.Users.FindAsync(userId);
            var emailBody = $@"
                <div style='font-family: sans-serif; max-width: 600px; padding: 20px; border: 1px solid #eee;'>
                    <h2 style='color: #c5a880; font-family: serif;'>AuraStay Reservation Confirmed!</h2>
                    <p>Dear {user?.Username},</p>
                    <p>Thank you for choosing AuraStay. Your reservation has been successfully booked and paid.</p>
                    
                    <div style='background: #fdfdfd; padding: 15px; margin: 20px 0; border-left: 4px solid #c5a880;'>
                        <p style='margin: 5px 0;'><strong>Reservation Code:</strong> {reservationNumber}</p>
                        <p style='margin: 5px 0;'><strong>Hotel:</strong> {room.Hotel?.Name}</p>
                        <p style='margin: 5px 0;'><strong>Location:</strong> {room.Hotel?.Location?.Name}, Bangalore</p>
                        <p style='margin: 5px 0;'><strong>Room Category:</strong> {room.RoomType} (x{request.RoomsCount})</p>
                        <p style='margin: 5px 0;'><strong>Check-In Date:</strong> {booking.CheckInDate.ToLongDateString()}</p>
                        <p style='margin: 5px 0;'><strong>Check-Out Date:</strong> {booking.CheckOutDate.ToLongDateString()}</p>
                        <p style='margin: 5px 0;'><strong>Duration:</strong> {nights} Nights</p>
                        <p style='margin: 5px 0;'><strong>Total Price Paid:</strong> &#x20B9;{finalPrice}</p>
                    </div>
                    
                    <p>We look forward to welcoming you soon.</p>
                    <p>Warm Regards,<br>Concierge Team, AuraStay</p>
                </div>";

            var emailLog = new EmailLog
            {
                BookingId = booking.Id,
                RecipientEmail = user?.Email ?? "guest@aurastay.com",
                Subject = $"AuraStay Reservation Confirmed - Code {reservationNumber}",
                Body = emailBody,
                SentAt = DateTime.UtcNow,
                Status = "Success"
            };
            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();

            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = userId,
                Username = user?.Username ?? "",
                RoomId = room.Id,
                RoomType = room.RoomType,
                HotelId = room.HotelId,
                HotelName = room.Hotel?.Name ?? "",
                HotelLocation = room.Hotel?.Location?.Name ?? "",
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                ReservationNumber = booking.ReservationNumber,
                Status = booking.Status,
                RoomsCount = booking.RoomsCount,
                CreatedAt = booking.CreatedAt
            };
        }

        public async Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r!.Hotel)
                .ThenInclude(h => h!.Location)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Username = b.User!.Username,
                    RoomId = b.RoomId,
                    RoomType = b.Room!.RoomType,
                    HotelId = b.Room.HotelId,
                    HotelName = b.Room.Hotel!.Name,
                    HotelLocation = b.Room.Hotel!.Location!.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    ReservationNumber = b.ReservationNumber,
                    Status = b.Status,
                    RoomsCount = b.RoomsCount,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r!.Hotel)
                .ThenInclude(h => h!.Location)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Username = b.User!.Username,
                    RoomId = b.RoomId,
                    RoomType = b.Room!.RoomType,
                    HotelId = b.Room.HotelId,
                    HotelName = b.Room.Hotel!.Name,
                    HotelLocation = b.Room.Hotel!.Location!.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    ReservationNumber = b.ReservationNumber,
                    Status = b.Status,
                    RoomsCount = b.RoomsCount,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId, bool isAdmin)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            if (!isAdmin && booking.UserId != userId) return false;
            if (booking.Status == "Cancelled") return false;

            booking.Status = "Cancelled";

            // If cancelled, deduct points earned from this booking (points = 10% of total price)
            var loyaltyRecord = await _context.LoyaltyRewards.FirstOrDefaultAsync(lr => lr.UserId == booking.UserId);
            if (loyaltyRecord != null)
            {
                int pointsEarned = (int)(booking.TotalPrice / 1000) * 100;
                loyaltyRecord.TotalPoints = Math.Max(0, loyaltyRecord.TotalPoints - pointsEarned);
                loyaltyRecord.AvailablePoints = Math.Max(0, loyaltyRecord.AvailablePoints - pointsEarned);
                loyaltyRecord.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EmailLog?> GetEmailLogByBookingIdAsync(int bookingId, int userId, bool isAdmin)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return null;

            if (!isAdmin && booking.UserId != userId) return null;

            return await _context.EmailLogs
                .FirstOrDefaultAsync(el => el.BookingId == bookingId);
        }

        private string GenerateReservationNumber()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }
    }
}
