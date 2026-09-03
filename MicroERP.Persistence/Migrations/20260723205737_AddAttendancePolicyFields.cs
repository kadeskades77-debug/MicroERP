using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendancePolicyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaximumPerformanceScore",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumWorkMinutes",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PartialAttendancePenaltyPoints",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumPerformanceScore",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "MinimumWorkMinutes",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "PartialAttendancePenaltyPoints",
                table: "AttendancePolicies");
        }
    }
}
