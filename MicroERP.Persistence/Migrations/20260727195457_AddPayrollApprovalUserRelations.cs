using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollApprovalUserRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaidByUserId",
                table: "Payrolls",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "Payrolls",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "Payrolls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_ApprovedByUserId",
                table: "Payrolls",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_PaidByUserId",
                table: "Payrolls",
                column: "PaidByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_ApprovedByUserId",
                table: "Payrolls",
                column: "ApprovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_PaidByUserId",
                table: "Payrolls",
                column: "PaidByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_AspNetUsers_ApprovedByUserId",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_AspNetUsers_PaidByUserId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_ApprovedByUserId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_PaidByUserId",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "Payrolls");

            migrationBuilder.AlterColumn<string>(
                name: "PaidByUserId",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
