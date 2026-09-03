using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReOrderOvertime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "EmployeeOvertimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "EmployeeOvertimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EmployeeOvertimes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmployeeOvertimes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "EmployeeOvertimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "EmployeeOvertimes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "EmployeeOvertimes");
        }
    }
}
