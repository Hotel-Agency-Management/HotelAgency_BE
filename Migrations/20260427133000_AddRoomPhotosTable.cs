using Booking.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260427133000_AddRoomPhotosTable")]
    public partial class AddRoomPhotosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `RoomPhotos` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `RoomId` int NOT NULL,
                    `PhotoUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    CONSTRAINT `PK_RoomPhotos` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_RoomPhotos_Rooms_RoomId`
                        FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`) ON DELETE CASCADE,
                    INDEX `IX_RoomPhotos_RoomId` (`RoomId`)
                ) CHARACTER SET=utf8mb4;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
