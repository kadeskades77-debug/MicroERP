using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmergencyAndSickdDaysToEmployeeLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidDays",
                table: "EmployeeLeaves",
                newName: "SickDays");

            migrationBuilder.AddColumn<int>(
                name: "EmergencyDays",
                table: "EmployeeLeaves",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyDays",
                table: "EmployeeLeaves");

            migrationBuilder.RenameColumn(
                name: "SickDays",
                table: "EmployeeLeaves",
                newName: "PaidDays");
        }
    }
}
