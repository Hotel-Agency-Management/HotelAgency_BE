using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdAndAgencyIdToPaymentLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgencyId",
                table: "PaymentLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "PaymentLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_AgencyId",
                table: "PaymentLogs",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_HotelId",
                table: "PaymentLogs",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLogs_Agencies_AgencyId",
                table: "PaymentLogs",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLogs_Hotels_HotelId",
                table: "PaymentLogs",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentLogs_Agencies_AgencyId",
                table: "PaymentLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentLogs_Hotels_HotelId",
                table: "PaymentLogs");

            migrationBuilder.DropIndex(
                name: "IX_PaymentLogs_AgencyId",
                table: "PaymentLogs");

            migrationBuilder.DropIndex(
                name: "IX_PaymentLogs_HotelId",
                table: "PaymentLogs");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "PaymentLogs");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "PaymentLogs");
        }
    }
}
