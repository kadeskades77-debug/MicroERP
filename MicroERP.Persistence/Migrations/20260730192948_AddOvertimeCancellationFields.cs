using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOvertimeCancellationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "EmployeeOvertimes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUserId",
                table: "EmployeeOvertimes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledOn",
                table: "EmployeeOvertimes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimes_CancelledByUserId",
                table: "EmployeeOvertimes",
                column: "CancelledByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOvertimes_AspNetUsers_CancelledByUserId",
                table: "EmployeeOvertimes",
                column: "CancelledByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOvertimes_AspNetUsers_CancelledByUserId",
                table: "EmployeeOvertimes");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimes_CancelledByUserId",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "EmployeeOvertimes");

            migrationBuilder.DropColumn(
                name: "CancelledOn",
                table: "EmployeeOvertimes");
        }
    }
}
