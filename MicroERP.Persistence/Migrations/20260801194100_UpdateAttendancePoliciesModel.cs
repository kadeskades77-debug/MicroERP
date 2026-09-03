using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttendancePoliciesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnnualLeavePenaltyPoints",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EarlyLeaveMinutesPerPenaltyPoint",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyAnnualLeaveLimit",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlySickLeaveLimit",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SickLeavePenaltyPoints",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnpaidLeavePenaltyPoints",
                table: "AttendancePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualLeavePenaltyPoints",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "EarlyLeaveMinutesPerPenaltyPoint",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "MonthlyAnnualLeaveLimit",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "MonthlySickLeaveLimit",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "SickLeavePenaltyPoints",
                table: "AttendancePolicies");

            migrationBuilder.DropColumn(
                name: "UnpaidLeavePenaltyPoints",
                table: "AttendancePolicies");
        }
    }
}
