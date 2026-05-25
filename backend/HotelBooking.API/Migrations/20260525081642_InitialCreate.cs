using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBooking.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeasonalOffers",
                columns: table => new
                {
                    OfferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicableLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicableHotel = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonalOffers", x => x.OfferId);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelAmenities",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelAmenities", x => new { x.HotelId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_HotelAmenities_Amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "Amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelAmenities_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedHotelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Hotels_AssignedHotelId",
                        column: x => x.AssignedHotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAvailabilities_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PointsRedeemed = table.Column<int>(type: "int", nullable: false),
                    ReservationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyRewards",
                columns: table => new
                {
                    RewardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    RedeemedPoints = table.Column<int>(type: "int", nullable: false),
                    AvailablePoints = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyRewards", x => x.RewardId);
                    table.ForeignKey(
                        name: "FK_LoyaltyRewards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPromotionUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPromotionUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPromotionUsages_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPromotionUsages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Amenities",
                columns: new[] { "Id", "Icon", "Name" },
                values: new object[,]
                {
                    { 1, "bi-wifi", "Free High-Speed Wi-Fi" },
                    { 2, "bi-water", "Swimming Pool" },
                    { 3, "bi-activity", "Fitness Center / Gym" },
                    { 4, "bi-heart-pulse", "Luxury Spa" },
                    { 5, "bi-egg-fried", "Complimentary Breakfast" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Whitefield" },
                    { 2, "Electronic City" },
                    { 3, "Koramangala" },
                    { 4, "Indiranagar" },
                    { 5, "MG Road" },
                    { 6, "Yeshwanthpur" }
                });

            migrationBuilder.InsertData(
                table: "Promotions",
                columns: new[] { "Id", "Active", "Code", "DiscountType", "DiscountValue", "ExpiryDate" },
                values: new object[,]
                {
                    { 1, true, "FIRST500", "Flat", 500m, new DateTime(2028, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, true, "BLR10", "Percentage", 10m, new DateTime(2028, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Admin" },
                    { 3, "User" }
                });

            migrationBuilder.InsertData(
                table: "SeasonalOffers",
                columns: new[] { "OfferId", "ApplicableHotel", "ApplicableLocation", "Description", "DiscountType", "DiscountValue", "EndDate", "IsActive", "StartDate", "Title" },
                values: new object[,]
                {
                    { 1, null, null, "Escape the heat with 20% off all bookings!", "Percentage", 20m, new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Summer Sale" },
                    { 2, null, null, "Get flat ₹1000 off during the rainy season!", "Flat", 1000m, new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monsoon Special" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Description", "ImageUrl", "LocationId", "Name", "Rating" },
                values: new object[,]
                {
                    { 1, "Experience prime comfort and boutique hospitality at Whitefield Grand Hotel in the heart of Whitefield, Bangalore.", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80", 1, "Whitefield Grand Hotel", 4.0 },
                    { 2, "Experience prime comfort and boutique hospitality at Royal Orchid Whitefield in the heart of Whitefield, Bangalore.", "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80", 1, "Royal Orchid Whitefield", 4.2000000000000002 },
                    { 3, "Experience prime comfort and boutique hospitality at ITPL Comfort Stay in the heart of Whitefield, Bangalore.", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80", 1, "ITPL Comfort Stay", 4.4000000000000004 },
                    { 4, "Experience prime comfort and boutique hospitality at Silver Oak Residency in the heart of Whitefield, Bangalore.", "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80", 1, "Silver Oak Residency", 4.5999999999999996 },
                    { 5, "Experience prime comfort and boutique hospitality at Electronic City Inn in the heart of Electronic City, Bangalore.", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80", 2, "Electronic City Inn", 4.7999999999999998 },
                    { 6, "Experience prime comfort and boutique hospitality at Tech Park Residency in the heart of Electronic City, Bangalore.", "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80", 2, "Tech Park Residency", 4.0 },
                    { 7, "Experience prime comfort and boutique hospitality at Silicon Stay Hotel in the heart of Electronic City, Bangalore.", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80", 2, "Silicon Stay Hotel", 4.2000000000000002 },
                    { 8, "Experience prime comfort and boutique hospitality at Green Valley Suites in the heart of Electronic City, Bangalore.", "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80", 2, "Green Valley Suites", 4.4000000000000004 },
                    { 9, "Experience prime comfort and boutique hospitality at Koramangala Comforts in the heart of Koramangala, Bangalore.", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80", 3, "Koramangala Comforts", 4.5999999999999996 },
                    { 10, "Experience prime comfort and boutique hospitality at Urban Nest Hotel in the heart of Koramangala, Bangalore.", "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80", 3, "Urban Nest Hotel", 4.7999999999999998 },
                    { 11, "Experience prime comfort and boutique hospitality at Royal Koramangala Suites in the heart of Koramangala, Bangalore.", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80", 3, "Royal Koramangala Suites", 4.0 },
                    { 12, "Experience prime comfort and boutique hospitality at Forum View Residency in the heart of Koramangala, Bangalore.", "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80", 3, "Forum View Residency", 4.2000000000000002 },
                    { 13, "Experience prime comfort and boutique hospitality at Indiranagar Grand Stay in the heart of Indiranagar, Bangalore.", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80", 4, "Indiranagar Grand Stay", 4.4000000000000004 },
                    { 14, "Experience prime comfort and boutique hospitality at Metro Comfort Hotel in the heart of Indiranagar, Bangalore.", "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80", 4, "Metro Comfort Hotel", 4.5999999999999996 },
                    { 15, "Experience prime comfort and boutique hospitality at 100 Feet Road Residency in the heart of Indiranagar, Bangalore.", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80", 4, "100 Feet Road Residency", 4.7999999999999998 },
                    { 16, "Experience prime comfort and boutique hospitality at Elite City Suites in the heart of Indiranagar, Bangalore.", "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80", 4, "Elite City Suites", 4.0 },
                    { 17, "Experience prime comfort and boutique hospitality at MG Road Palace Hotel in the heart of MG Road, Bangalore.", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80", 5, "MG Road Palace Hotel", 4.2000000000000002 },
                    { 18, "Experience prime comfort and boutique hospitality at Brigade View Residency in the heart of MG Road, Bangalore.", "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80", 5, "Brigade View Residency", 4.4000000000000004 },
                    { 19, "Experience prime comfort and boutique hospitality at Central Grand Hotel in the heart of MG Road, Bangalore.", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80", 5, "Central Grand Hotel", 4.5999999999999996 },
                    { 20, "Experience prime comfort and boutique hospitality at City Lights Suites in the heart of MG Road, Bangalore.", "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80", 5, "City Lights Suites", 4.7999999999999998 },
                    { 21, "Experience prime comfort and boutique hospitality at Yeshwanthpur Comfort Inn in the heart of Yeshwanthpur, Bangalore.", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=600&q=80", 6, "Yeshwanthpur Comfort Inn", 4.0 },
                    { 22, "Experience prime comfort and boutique hospitality at Railway View Hotel in the heart of Yeshwanthpur, Bangalore.", "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=600&q=80", 6, "Railway View Hotel", 4.2000000000000002 },
                    { 23, "Experience prime comfort and boutique hospitality at Orion Grand Residency in the heart of Yeshwanthpur, Bangalore.", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=600&q=80", 6, "Orion Grand Residency", 4.4000000000000004 },
                    { 24, "Experience prime comfort and boutique hospitality at Metro Stay Suites in the heart of Yeshwanthpur, Bangalore.", "https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=600&q=80", 6, "Metro Stay Suites", 4.5999999999999996 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AssignedHotelId", "Email", "PasswordHash", "RoleId", "Username" },
                values: new object[,]
                {
                    { 1, null, "superadmin@aurastay.com", "01NbeOJIZ/PIUP7ByFkbekabj4aDvmTYNQV8uf0gSqE=", 1, "superadmin" },
                    { 3, null, "user@hotelbooking.com", "PnwZV2SIhigW8TtRLKzz5LqX3ZckPqC9airRZC2GunI=", 3, "john_doe" }
                });

            migrationBuilder.InsertData(
                table: "HotelAmenities",
                columns: new[] { "AmenityId", "HotelId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 1 },
                    { 5, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 2 },
                    { 1, 3 },
                    { 3, 3 },
                    { 5, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 3, 4 },
                    { 4, 4 },
                    { 5, 4 },
                    { 1, 5 },
                    { 3, 5 },
                    { 5, 5 },
                    { 1, 6 },
                    { 2, 6 },
                    { 3, 6 },
                    { 4, 6 },
                    { 5, 6 },
                    { 1, 7 },
                    { 3, 7 },
                    { 5, 7 },
                    { 1, 8 },
                    { 2, 8 },
                    { 3, 8 },
                    { 4, 8 },
                    { 5, 8 },
                    { 1, 9 },
                    { 3, 9 },
                    { 5, 9 },
                    { 1, 10 },
                    { 2, 10 },
                    { 3, 10 },
                    { 4, 10 },
                    { 5, 10 },
                    { 1, 11 },
                    { 3, 11 },
                    { 5, 11 },
                    { 1, 12 },
                    { 2, 12 },
                    { 3, 12 },
                    { 4, 12 },
                    { 5, 12 },
                    { 1, 13 },
                    { 3, 13 },
                    { 5, 13 },
                    { 1, 14 },
                    { 2, 14 },
                    { 3, 14 },
                    { 4, 14 },
                    { 5, 14 },
                    { 1, 15 },
                    { 3, 15 },
                    { 5, 15 },
                    { 1, 16 },
                    { 2, 16 },
                    { 3, 16 },
                    { 4, 16 },
                    { 5, 16 },
                    { 1, 17 },
                    { 3, 17 },
                    { 5, 17 },
                    { 1, 18 },
                    { 2, 18 },
                    { 3, 18 },
                    { 4, 18 },
                    { 5, 18 },
                    { 1, 19 },
                    { 3, 19 },
                    { 5, 19 },
                    { 1, 20 },
                    { 2, 20 },
                    { 3, 20 },
                    { 4, 20 },
                    { 5, 20 },
                    { 1, 21 },
                    { 3, 21 },
                    { 5, 21 },
                    { 1, 22 },
                    { 2, 22 },
                    { 3, 22 },
                    { 4, 22 },
                    { 5, 22 },
                    { 1, 23 },
                    { 3, 23 },
                    { 5, 23 },
                    { 1, 24 },
                    { 2, 24 },
                    { 3, 24 },
                    { 4, 24 },
                    { 5, 24 }
                });

            migrationBuilder.InsertData(
                table: "LoyaltyRewards",
                columns: new[] { "RewardId", "AvailablePoints", "LastUpdated", "RedeemedPoints", "TotalPoints", "UserId" },
                values: new object[] { 1, 350, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), 0, 350, 3 });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "Description", "HotelId", "ImageUrl", "PricePerNight", "RoomType", "Status" },
                values: new object[,]
                {
                    { 1, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 1, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1260m, "normal room", "Available" },
                    { 2, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 1, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 2600m, "super deluxe room", "Available" },
                    { 3, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 1, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 1880m, "one AC room", "Available" },
                    { 4, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 2, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1320m, "normal room", "Available" },
                    { 5, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 2, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 2700m, "super deluxe room", "Available" },
                    { 6, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 2, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 1960m, "one AC room", "Available" },
                    { 7, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 3, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1380m, "normal room", "Available" },
                    { 8, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 3, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 2800m, "super deluxe room", "Available" },
                    { 9, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 3, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2040m, "one AC room", "Available" },
                    { 10, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 4, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1440m, "normal room", "Available" },
                    { 11, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 4, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 2900m, "super deluxe room", "Available" },
                    { 12, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 4, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2120m, "one AC room", "Available" },
                    { 13, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 5, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1500m, "normal room", "Available" },
                    { 14, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 5, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3000m, "super deluxe room", "Available" },
                    { 15, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 5, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2200m, "one AC room", "Available" },
                    { 16, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 6, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1560m, "normal room", "Available" },
                    { 17, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 6, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3100m, "super deluxe room", "Available" },
                    { 18, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 6, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2280m, "one AC room", "Available" },
                    { 19, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 7, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1620m, "normal room", "Available" },
                    { 20, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 7, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3200m, "super deluxe room", "Available" },
                    { 21, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 7, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2360m, "one AC room", "Available" },
                    { 22, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 8, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1680m, "normal room", "Available" },
                    { 23, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 8, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3300m, "super deluxe room", "Available" },
                    { 24, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 8, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2440m, "one AC room", "Available" },
                    { 25, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 9, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1740m, "normal room", "Available" },
                    { 26, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 9, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3400m, "super deluxe room", "Available" },
                    { 27, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 9, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2520m, "one AC room", "Available" },
                    { 28, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 10, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1800m, "normal room", "Available" },
                    { 29, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 10, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3500m, "super deluxe room", "Available" },
                    { 30, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 10, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2600m, "one AC room", "Available" },
                    { 31, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 11, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1860m, "normal room", "Available" },
                    { 32, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 11, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3600m, "super deluxe room", "Available" },
                    { 33, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 11, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2680m, "one AC room", "Available" },
                    { 34, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 12, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1920m, "normal room", "Available" },
                    { 35, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 12, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3700m, "super deluxe room", "Available" },
                    { 36, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 12, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2760m, "one AC room", "Available" },
                    { 37, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 13, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 1980m, "normal room", "Available" },
                    { 38, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 13, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3800m, "super deluxe room", "Available" },
                    { 39, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 13, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2840m, "one AC room", "Available" },
                    { 40, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 14, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2040m, "normal room", "Available" },
                    { 41, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 14, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 3900m, "super deluxe room", "Available" },
                    { 42, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 14, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 2920m, "one AC room", "Available" },
                    { 43, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 15, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2100m, "normal room", "Available" },
                    { 44, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 15, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4000m, "super deluxe room", "Available" },
                    { 45, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 15, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3000m, "one AC room", "Available" },
                    { 46, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 16, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2160m, "normal room", "Available" },
                    { 47, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 16, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4100m, "super deluxe room", "Available" },
                    { 48, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 16, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3080m, "one AC room", "Available" },
                    { 49, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 17, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2220m, "normal room", "Available" },
                    { 50, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 17, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4200m, "super deluxe room", "Available" },
                    { 51, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 17, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3160m, "one AC room", "Available" },
                    { 52, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 18, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2280m, "normal room", "Available" },
                    { 53, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 18, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4300m, "super deluxe room", "Available" },
                    { 54, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 18, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3240m, "one AC room", "Available" },
                    { 55, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 19, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2340m, "normal room", "Available" },
                    { 56, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 19, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4400m, "super deluxe room", "Available" },
                    { 57, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 19, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3320m, "one AC room", "Available" },
                    { 58, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 20, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2400m, "normal room", "Available" },
                    { 59, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 20, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4500m, "super deluxe room", "Available" },
                    { 60, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 20, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3400m, "one AC room", "Available" },
                    { 61, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 21, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2460m, "normal room", "Available" },
                    { 62, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 21, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4600m, "super deluxe room", "Available" },
                    { 63, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 21, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3480m, "one AC room", "Available" },
                    { 64, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 22, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2520m, "normal room", "Available" },
                    { 65, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 22, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4700m, "super deluxe room", "Available" },
                    { 66, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 22, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3560m, "one AC room", "Available" },
                    { 67, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 23, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2580m, "normal room", "Available" },
                    { 68, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 23, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4800m, "super deluxe room", "Available" },
                    { 69, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 23, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3640m, "one AC room", "Available" },
                    { 70, 2, "Cozy minimalist layout with a plush queen-sized bed, work desk, and fresh air fan ventilation.", 24, "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=500&q=80", 2640m, "normal room", "Available" },
                    { 71, 3, "Spacious luxury premium room featuring a king-sized bed, city skyline windows, mini-lounge, and bar.", 24, "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=500&q=80", 4900m, "super deluxe room", "Available" },
                    { 72, 2, "Premium climate-controlled room featuring silent air conditioning, double bed, and workstation setup.", 24, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=500&q=80", 3720m, "one AC room", "Available" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AssignedHotelId", "Email", "PasswordHash", "RoleId", "Username" },
                values: new object[] { 2, 1, "admin_whitefield1@hotel.com", "6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=", 2, "hoteladmin_wf1" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_BookingId",
                table: "EmailLogs",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelAmenities_AmenityId",
                table: "HotelAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_LocationId",
                table: "Hotels",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyRewards_UserId",
                table: "LoyaltyRewards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAvailabilities_RoomId",
                table: "RoomAvailabilities",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPromotionUsages_PromotionId",
                table: "UserPromotionUsages",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPromotionUsages_UserId",
                table: "UserPromotionUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AssignedHotelId",
                table: "Users",
                column: "AssignedHotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "HotelAmenities");

            migrationBuilder.DropTable(
                name: "LoyaltyRewards");

            migrationBuilder.DropTable(
                name: "RoomAvailabilities");

            migrationBuilder.DropTable(
                name: "SeasonalOffers");

            migrationBuilder.DropTable(
                name: "UserPromotionUsages");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
