using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPartialAttendanceWithPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PartialAttendanceWithPermissionPercentage",
                table: "PayrollPolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartialAttendanceWithoutPermissionPercentage",
                table: "PayrollPolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartialAttendanceWithPermissionPercentage",
                table: "PayrollPolicies");

            migrationBuilder.DropColumn(
                name: "PartialAttendanceWithoutPermissionPercentage",
                table: "PayrollPolicies");
        }
    }
}
