using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class EdaitRoomAndRoomType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyPrice",
                table: "Rooms",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyPrice",
                table: "Rooms",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPrice",
                table: "Rooms",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtendPrice",
                table: "Rooms",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE `Rooms` r
                INNER JOIN `RoomTypes` rt ON rt.`Id` = r.`RoomTypeId`
                SET
                    r.`Capacity` = CASE WHEN r.`Capacity` = 0 THEN rt.`Capacity` ELSE r.`Capacity` END,
                    r.`DailyPrice` = CASE WHEN r.`DailyPrice` = 0 THEN rt.`DailyPrice` ELSE r.`DailyPrice` END,
                    r.`WeeklyPrice` = CASE WHEN r.`WeeklyPrice` = 0 THEN rt.`WeeklyPrice` ELSE r.`WeeklyPrice` END,
                    r.`MonthlyPrice` = CASE WHEN r.`MonthlyPrice` = 0 THEN rt.`MonthlyPrice` ELSE r.`MonthlyPrice` END,
                    r.`ExtendPrice` = CASE WHEN r.`ExtendPrice` = 0 THEN rt.`DailyPrice` ELSE r.`ExtendPrice` END;
                """);

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "DailyPrice",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "WeeklyPrice",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "RoomTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyPrice",
                table: "RoomTypes",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyPrice",
                table: "RoomTypes",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPrice",
                table: "RoomTypes",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE `RoomTypes` rt
                INNER JOIN (
                    SELECT
                        `RoomTypeId`,
                        MAX(`Capacity`) AS `Capacity`,
                        MAX(`DailyPrice`) AS `DailyPrice`,
                        MAX(`WeeklyPrice`) AS `WeeklyPrice`,
                        MAX(`MonthlyPrice`) AS `MonthlyPrice`
                    FROM `Rooms`
                    GROUP BY `RoomTypeId`
                ) room_values ON room_values.`RoomTypeId` = rt.`Id`
                SET
                    rt.`Capacity` = room_values.`Capacity`,
                    rt.`DailyPrice` = room_values.`DailyPrice`,
                    rt.`WeeklyPrice` = room_values.`WeeklyPrice`,
                    rt.`MonthlyPrice` = room_values.`MonthlyPrice`;
                """);

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DailyPrice",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "WeeklyPrice",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ExtendPrice",
                table: "Rooms");
        }
    }
}
