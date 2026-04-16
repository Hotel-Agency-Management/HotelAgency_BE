using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityPhotos_Facility_FacilityId",
                table: "FacilityPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_FeatureLimits_Hotels_HotelId",
                table: "FeatureLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_FeatureLimits_PlanFeatures_FeatureId",
                table: "FeatureLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomTypes_Hotels_HotelId",
                table: "RoomTypes");

            migrationBuilder.DropIndex(
                name: "IX_RoomTypes_HotelId",
                table: "RoomTypes");

            migrationBuilder.DropIndex(
                name: "IX_RoomTypes_Name_HotelId",
                table: "RoomTypes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Facility_TempId",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "FeatureLimits");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FeatureLimits");

            migrationBuilder.DropColumn(
                name: "OpenAt",
                table: "FeatureLimits");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FeatureLimits");

            migrationBuilder.RenameTable(
                name: "Facility",
                newName: "Facilities");

            migrationBuilder.RenameColumn(
                name: "TempId",
                table: "Facilities",
                newName: "HotelId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CloseAt",
                table: "Facilities",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Facilities",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Facilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FacilityType",
                table: "Facilities",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Facilities",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpenAt",
                table: "Facilities",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Facilities",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Facilities",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facilities",
                table: "Facilities",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_Name",
                table: "RoomTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_HotelId",
                table: "Facilities",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Hotels_HotelId",
                table: "Facilities",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityPhotos_Facilities_FacilityId",
                table: "FacilityPhotos",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureLimits_PlanFeatures_FeatureId",
                table: "FeatureLimits",
                column: "FeatureId",
                principalTable: "PlanFeatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Hotels_HotelId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityPhotos_Facilities_FacilityId",
                table: "FacilityPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_FeatureLimits_PlanFeatures_FeatureId",
                table: "FeatureLimits");

            migrationBuilder.DropIndex(
                name: "IX_RoomTypes_Name",
                table: "RoomTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facilities",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_HotelId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "CloseAt",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "FacilityType",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "OpenAt",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Facilities");

            migrationBuilder.RenameTable(
                name: "Facilities",
                newName: "Facility");

            migrationBuilder.RenameColumn(
                name: "HotelId",
                table: "Facility",
                newName: "TempId");

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "FeatureLimits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FeatureLimits",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpenAt",
                table: "FeatureLimits",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FeatureLimits",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Facility_TempId",
                table: "Facility",
                column: "TempId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_HotelId",
                table: "RoomTypes",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_Name_HotelId",
                table: "RoomTypes",
                columns: new[] { "Name", "HotelId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityPhotos_Facility_FacilityId",
                table: "FacilityPhotos",
                column: "FacilityId",
                principalTable: "Facility",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureLimits_Hotels_HotelId",
                table: "FeatureLimits",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureLimits_PlanFeatures_FeatureId",
                table: "FeatureLimits",
                column: "FeatureId",
                principalTable: "PlanFeatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomTypes_Hotels_HotelId",
                table: "RoomTypes",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
