using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class PlanIdToAgencyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Agencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_PlanId",
                table: "Agencies",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_Plans_PlanId",
                table: "Agencies",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_Plans_PlanId",
                table: "Agencies");

            migrationBuilder.DropIndex(
                name: "IX_Agencies_PlanId",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Agencies");
        }
    }
}
