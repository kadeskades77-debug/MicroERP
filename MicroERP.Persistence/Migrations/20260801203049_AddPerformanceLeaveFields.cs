using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceLeaveFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnnualLeaveDays",
                table: "AttendancePerformances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EarlyLeaveDays",
                table: "AttendancePerformances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SickLeaveDays",
                table: "AttendancePerformances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnpaidLeaveDays",
                table: "AttendancePerformances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualLeaveDays",
                table: "AttendancePerformances");

            migrationBuilder.DropColumn(
                name: "EarlyLeaveDays",
                table: "AttendancePerformances");

            migrationBuilder.DropColumn(
                name: "SickLeaveDays",
                table: "AttendancePerformances");

            migrationBuilder.DropColumn(
                name: "UnpaidLeaveDays",
                table: "AttendancePerformances");
        }
    }
}
