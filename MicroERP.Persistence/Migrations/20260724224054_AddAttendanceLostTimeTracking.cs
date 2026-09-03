using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceLostTimeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpectedMinutes",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LostTimeMinutes",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LostMinutesPerPenaltyPoint",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLostTimeMinutes",
                table: "AttendancePerformances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedMinutes",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LostTimeMinutes",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LostMinutesPerPenaltyPoint",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "TotalLostTimeMinutes",
                table: "AttendancePerformances");
        }
    }
}
