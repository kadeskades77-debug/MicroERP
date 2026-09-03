using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addSuspendedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedByUserId",
                table: "EmployeeLoans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLoans_SuspendedByUserId",
                table: "EmployeeLoans",
                column: "SuspendedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLoans_AspNetUsers_SuspendedByUserId",
                table: "EmployeeLoans",
                column: "SuspendedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLoans_AspNetUsers_SuspendedByUserId",
                table: "EmployeeLoans");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLoans_SuspendedByUserId",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "SuspendedByUserId",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                table: "EmployeeLoans");
        }
    }
}
