using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "EmployeeEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "EmployeeEvaluations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "EmployeeEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedOn",
                table: "EmployeeEvaluations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "EmployeeEvaluations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "EmployeeEvaluations");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "EmployeeEvaluations");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "EmployeeEvaluations");

            migrationBuilder.DropColumn(
                name: "RejectedOn",
                table: "EmployeeEvaluations");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "EmployeeEvaluations");
        }
    }
}
