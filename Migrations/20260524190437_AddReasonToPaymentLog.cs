using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonToPaymentLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET @dbname = DATABASE();
                SET @tablename = 'PaymentLogs';
                SET @columnname = 'Reason';
                SET @preparedStatement = (
                    SELECT IF(
                        (SELECT COUNT(*) FROM information_schema.COLUMNS
                         WHERE TABLE_SCHEMA = @dbname
                           AND TABLE_NAME  = @tablename
                           AND COLUMN_NAME = @columnname) > 0,
                        'SELECT 1',
                        CONCAT('ALTER TABLE `', @tablename, '` ADD `', @columnname, '` longtext CHARACTER SET utf8mb4 NULL')
                    )
                );
                PREPARE addColumnIfNotExists FROM @preparedStatement;
                EXECUTE addColumnIfNotExists;
                DEALLOCATE PREPARE addColumnIfNotExists;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "PaymentLogs");
        }
    }
}
