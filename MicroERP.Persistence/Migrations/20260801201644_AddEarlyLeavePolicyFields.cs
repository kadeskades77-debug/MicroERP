using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEarlyLeavePolicyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EarlyLeaveMinutesPerPenaltyPoint",
                table: "AttendancePolicies",
                newName: "MonthlyEarlyLeaveLimit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthlyEarlyLeaveLimit",
                table: "AttendancePolicies",
                newName: "EarlyLeaveMinutesPerPenaltyPoint");
        }
    }
}
