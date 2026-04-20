using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class AddNavegation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `RoomTypes` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    `Description` longtext CHARACTER SET utf8mb4 NULL,
                    `Capacity` int NOT NULL,
                    `DailyPrice` decimal(65,30) NOT NULL,
                    `WeeklyPrice` decimal(65,30) NOT NULL,
                    `MonthlyPrice` decimal(65,30) NOT NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    `UpdatedAt` datetime(6) NOT NULL,
                    CONSTRAINT `PK_RoomTypes` PRIMARY KEY (`Id`),
                    UNIQUE INDEX `IX_RoomTypes_Name` (`Name`)
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `RoomAmenities` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    CONSTRAINT `PK_RoomAmenities` PRIMARY KEY (`Id`),
                    UNIQUE INDEX `IX_RoomAmenities_Name` (`Name`)
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `Rooms` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `HotelId` int NOT NULL,
                    `RoomTypeId` int NOT NULL,
                    `RoomNumber` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    `FloorNumber` int NOT NULL,
                    `Description` longtext CHARACTER SET utf8mb4 NULL,
                    `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
                    `Notes` longtext CHARACTER SET utf8mb4 NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    `UpdatedAt` datetime(6) NOT NULL,
                    `CoverPhotoUrl` longtext CHARACTER SET utf8mb4 NULL,
                    CONSTRAINT `PK_Rooms` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Rooms_Hotels_HotelId`
                        FOREIGN KEY (`HotelId`) REFERENCES `Hotels` (`Id`) ON DELETE CASCADE,
                    CONSTRAINT `FK_Rooms_RoomTypes_RoomTypeId`
                        FOREIGN KEY (`RoomTypeId`) REFERENCES `RoomTypes` (`Id`) ON DELETE RESTRICT,
                    INDEX `IX_Rooms_HotelId` (`HotelId`),
                    UNIQUE INDEX `IX_Rooms_RoomNumber_HotelId` (`RoomNumber`, `HotelId`),
                    INDEX `IX_Rooms_RoomTypeId` (`RoomTypeId`)
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `RoomAmenityMaps` (
                    `AmenitiesId` int NOT NULL,
                    `RoomsId` int NOT NULL,
                    CONSTRAINT `PK_RoomAmenityMaps` PRIMARY KEY (`AmenitiesId`, `RoomsId`),
                    CONSTRAINT `FK_RoomAmenityMaps_RoomAmenities_AmenitiesId`
                        FOREIGN KEY (`AmenitiesId`) REFERENCES `RoomAmenities` (`Id`) ON DELETE CASCADE,
                    CONSTRAINT `FK_RoomAmenityMaps_Rooms_RoomsId`
                        FOREIGN KEY (`RoomsId`) REFERENCES `Rooms` (`Id`) ON DELETE CASCADE,
                    INDEX `IX_RoomAmenityMaps_RoomsId` (`RoomsId`)
                ) CHARACTER SET=utf8mb4;
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomAmenityMaps");
        }
    }
}
