using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Models;

namespace HotelBooking.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomAvailability> RoomAvailabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<HotelAmenity> HotelAmenities { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<UserPromotionUsage> UserPromotionUsages { get; set; }
        public DbSet<LoyaltyReward> LoyaltyRewards { get; set; }
        public DbSet<SeasonalOffer> SeasonalOffers { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.AssignedHotel)
                .WithMany()
                .HasForeignKey(u => u.AssignedHotelId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Location)
                .WithMany(l => l.Hotels)
                .HasForeignKey(h => h.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomAvailability>()
                .HasOne(ra => ra.Room)
                .WithMany()
                .HasForeignKey(ra => ra.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailLog>()
                .HasOne(el => el.Booking)
                .WithMany()
                .HasForeignKey(el => el.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many Join for Hotel and Amenity
            modelBuilder.Entity<HotelAmenity>()
                .HasKey(ha => new { ha.HotelId, ha.AmenityId });

            modelBuilder.Entity<HotelAmenity>()
                .HasOne(ha => ha.Hotel)
                .WithMany(h => h.HotelAmenities)
                .HasForeignKey(ha => ha.HotelId);

            modelBuilder.Entity<HotelAmenity>()
                .HasOne(ha => ha.Amenity)
                .WithMany(a => a.HotelAmenities)
                .HasForeignKey(ha => ha.AmenityId);

            // Decimal Precision Config
            modelBuilder.Entity<Room>().Property(r => r.PricePerNight).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Booking>().Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Promotion>().Property(p => p.DiscountValue).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SeasonalOffer>().Property(s => s.DiscountValue).HasColumnType("decimal(18,2)");

            // Seed Roles: SuperAdmin (1), Admin (2), User (3)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "User" }
            );

            // Seed Default Accounts
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "superadmin", Email = "superadmin@aurastay.com", PasswordHash = PasswordHasher.HashPassword("SuperAdmin@123"), RoleId = 1 },
                new User { Id = 2, Username = "hoteladmin_wf1", Email = "admin_whitefield1@hotel.com", PasswordHash = PasswordHasher.HashPassword("Admin@123"), RoleId = 2, AssignedHotelId = 1 },
                new User { Id = 3, Username = "john_doe", Email = "user@hotelbooking.com", PasswordHash = PasswordHasher.HashPassword("User@123"), RoleId = 3 }
            );

            // Seed Loyalty Points balance for User (Id = 3)
            modelBuilder.Entity<LoyaltyReward>().HasData(
                new LoyaltyReward { RewardId = 1, UserId = 3, TotalPoints = 350, RedeemedPoints = 0, AvailablePoints = 350, LastUpdated = new DateTime(2026, 5, 25, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Seed 6 Locations in Bangalore
            var locations = new[] { "Whitefield", "Electronic City", "Koramangala", "Indiranagar", "MG Road", "Yeshwanthpur" };
            for (int i = 0; i < locations.Length; i++)
            {
                modelBuilder.Entity<Location>().HasData(new Location { Id = i + 1, Name = locations[i] });
            }

            // Seed 5 Core Amenities
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity { Id = 1, Name = "Free High-Speed Wi-Fi", Icon = "bi-wifi" },
                new Amenity { Id = 2, Name = "Swimming Pool", Icon = "bi-water" },
                new Amenity { Id = 3, Name = "Fitness Center / Gym", Icon = "bi-activity" },
                new Amenity { Id = 4, Name = "Luxury Spa", Icon = "bi-heart-pulse" },
                new Amenity { Id = 5, Name = "Complimentary Breakfast", Icon = "bi-egg-fried" }
            );

            // Seed 24 Bangalore Hotels (4 per location) and 48 Rooms (2 per hotel)
            var hotelNames = new[] {
                // Whitefield
                "Whitefield Grand Hotel", "Royal Orchid Whitefield", "ITPL Comfort Stay", "Silver Oak Residency",
                // Electronic City
                "Electronic City Inn", "Tech Park Residency", "Silicon Stay Hotel", "Green Valley Suites",
                // Koramangala
                "Koramangala Comforts", "Urban Nest Hotel", "Royal Koramangala Suites", "Forum View Residency",
                // Indiranagar
                "Indiranagar Grand Stay", "Metro Comfort Hotel", "100 Feet Road Residency", "Elite City Suites",
                // MG Road
                "MG Road Palace Hotel", "Brigade View Residency", "Central Grand Hotel", "City Lights Suites",
                // Yeshwanthpur
                "Yeshwanthpur Comfort Inn", "Railway View Hotel", "Orion Grand Residency", "Metro Stay Suites"
            };

            var hotelImages = new[] {
                "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80",
                "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80",
                "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80",
                "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80"
            };

            var roomTypeImages = new[] {
                "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80",
                "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80",
                "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80"
            };

            var hotelList = new List<Hotel>();
            var roomList = new List<Room>();
            var hotelAmenityList = new List<HotelAmenity>();

            int roomIdCounter = 1;
            for (int i = 0; i < hotelNames.Length; i++)
            {
                int hotelId = i + 1;
                int locId = (i / 4) + 1;
                string imgUrl = hotelImages[i % 4];

                hotelList.Add(new Hotel
                {
                    Id = hotelId,
                    Name = hotelNames[i],
                    LocationId = locId,
                    Description = $"Experience prime comfort and boutique hospitality at {hotelNames[i]} in the heart of {locations[locId - 1]}, Bangalore.",
                    ImageUrl = imgUrl,
                    Rating = Math.Round((4.0 + (i % 5) * 0.2) * 10) / 10
                });

                // normal room
                roomList.Add(new Room
                {
                    Id = roomIdCounter++,
                    HotelId = hotelId,
                    RoomType = "normal room",
                    PricePerNight = 1200 + (hotelId * 60),
                    Capacity = 2,
                    Description = "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.",
                    ImageUrl = roomTypeImages[0],
                    Status = "Available"
                });

                // super deluxe room
                roomList.Add(new Room
                {
                    Id = roomIdCounter++,
                    HotelId = hotelId,
                    RoomType = "super deluxe room",
                    PricePerNight = 2500 + (hotelId * 100),
                    Capacity = 3,
                    Description = "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.",
                    ImageUrl = roomTypeImages[1],
                    Status = "Available"
                });

                // one AC room
                roomList.Add(new Room
                {
                    Id = roomIdCounter++,
                    HotelId = hotelId,
                    RoomType = "one AC room",
                    PricePerNight = 1800 + (hotelId * 80),
                    Capacity = 2,
                    Description = "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.",
                    ImageUrl = roomTypeImages[2],
                    Status = "Available"
                });

                // Map Amenities
                hotelAmenityList.Add(new HotelAmenity { HotelId = hotelId, AmenityId = 1 });
                hotelAmenityList.Add(new HotelAmenity { HotelId = hotelId, AmenityId = 3 });
                hotelAmenityList.Add(new HotelAmenity { HotelId = hotelId, AmenityId = 5 });
                if (hotelId % 2 == 0)
                {
                    hotelAmenityList.Add(new HotelAmenity { HotelId = hotelId, AmenityId = 2 });
                    hotelAmenityList.Add(new HotelAmenity { HotelId = hotelId, AmenityId = 4 });
                }
            }

            modelBuilder.Entity<Hotel>().HasData(hotelList);
            modelBuilder.Entity<Room>().HasData(roomList);
            modelBuilder.Entity<HotelAmenity>().HasData(hotelAmenityList);

            // Seed Discount Coupons
            modelBuilder.Entity<Promotion>().HasData(
                new Promotion { Id = 1, Code = "FIRST500", DiscountType = "Flat", DiscountValue = 500, Active = true, ExpiryDate = new DateTime(2028, 5, 25) },
                new Promotion { Id = 2, Code = "BLR10", DiscountType = "Percentage", DiscountValue = 10, Active = true, ExpiryDate = new DateTime(2028, 5, 25) }
            );

            // Seed Seasonal Offers
            modelBuilder.Entity<SeasonalOffer>().HasData(
                new SeasonalOffer { OfferId = 1, Title = "Summer Sale", Description = "Escape the heat with 20% off all bookings!", DiscountType = "Percentage", DiscountValue = 20, StartDate = new DateTime(2026, 5, 1), EndDate = new DateTime(2026, 8, 31), IsActive = true },
                new SeasonalOffer { OfferId = 2, Title = "Monsoon Special", Description = "Get flat ₹1000 off during the rainy season!", DiscountType = "Flat", DiscountValue = 1000, StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 11, 30), IsActive = true }
            );
        }
    }
}
