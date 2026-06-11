using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActorId = table.Column<int>(type: "int", nullable: true),
                    ActorName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActorRole = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AgencyId = table.Column<int>(type: "int", nullable: true),
                    HotelId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_system_logs_Users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_system_logs_Action",
                table: "system_logs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_system_logs_ActorId",
                table: "system_logs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_system_logs_CreatedAt",
                table: "system_logs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_system_logs_EntityType",
                table: "system_logs",
                column: "EntityType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_logs");
        }
    }
}
