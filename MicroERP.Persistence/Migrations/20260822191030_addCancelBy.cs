using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addCancelBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledByUserId",
                table: "EmployeeLoans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLoans_CancelledByUserId",
                table: "EmployeeLoans",
                column: "CancelledByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLoans_AspNetUsers_CancelledByUserId",
                table: "EmployeeLoans",
                column: "CancelledByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLoans_AspNetUsers_CancelledByUserId",
                table: "EmployeeLoans");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLoans_CancelledByUserId",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "EmployeeLoans");
        }
    }
}
