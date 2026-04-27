using Booking.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260427120000_AddFacilitiesTable")]
    public partial class AddFacilitiesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `Facilities` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `HotelId` int NOT NULL,
                    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `FacilityType` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Description` longtext CHARACTER SET utf8mb4 NULL,
                    `Status` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
                    `OpenAt` time(6) NULL,
                    `CloseAt` time(6) NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    `UpdatedAt` datetime(6) NOT NULL,
                    CONSTRAINT `PK_Facilities` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Facilities_Hotels_HotelId`
                        FOREIGN KEY (`HotelId`) REFERENCES `Hotels` (`Id`) ON DELETE CASCADE,
                    INDEX `IX_Facilities_HotelId` (`HotelId`)
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `FacilityPhotos` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `FacilityId` int NOT NULL,
                    `PhotoUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    CONSTRAINT `PK_FacilityPhotos` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_FacilityPhotos_Facilities_FacilityId`
                        FOREIGN KEY (`FacilityId`) REFERENCES `Facilities` (`Id`) ON DELETE CASCADE,
                    INDEX `IX_FacilityPhotos_FacilityId` (`FacilityId`)
                ) CHARACTER SET=utf8mb4;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
