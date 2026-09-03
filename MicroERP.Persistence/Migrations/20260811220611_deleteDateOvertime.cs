using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class deleteDateOvertime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimes_EmployeeId_Date",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "Date",
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

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimes_EmployeeId_StartDateTime",
                table: "EmployeeOvertimes",
                columns: new[] { "EmployeeId", "StartDateTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimes_EmployeeId_StartDateTime",
                table: "EmployeeOvertimes");

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

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "EmployeeOvertimes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

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

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimes_EmployeeId_Date",
                table: "EmployeeOvertimes",
                columns: new[] { "EmployeeId", "Date" });
        }
    }
}
