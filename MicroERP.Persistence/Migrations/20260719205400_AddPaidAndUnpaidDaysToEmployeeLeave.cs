using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaidAndUnpaidDaysToEmployeeLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<bool>(
                name: "AllowNegativeBalance",
                table: "LeavePolicies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxNegativeDays",
                table: "LeavePolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaidDays",
                table: "EmployeeLeaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnpaidDays",
                table: "EmployeeLeaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "LeavePolicies",
                columns: new[] { "Id", "AllowNegativeBalance", "CreatedBy", "CreatedOn", "DaysPerMonth", "IsActive", "IsDeleted", "LeaveType", "MaxNegativeDays", "MaximumDaysPerYear", "ModifiedBy", "ModifiedOn" },
                values: new object[,]
                {
                    { -4, true, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 4, 0, 0, null, null },
                    { -3, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 3, 0, 10, null, null },
                    { -2, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2m, true, false, 2, 0, 22, null, null },
                    { -1, true, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.5m, true, false, 1, 10, 30, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DropColumn(
                name: "AllowNegativeBalance",
                table: "LeavePolicies");

            migrationBuilder.DropColumn(
                name: "MaxNegativeDays",
                table: "LeavePolicies");

            migrationBuilder.DropColumn(
                name: "PaidDays",
                table: "EmployeeLeaves");

            migrationBuilder.DropColumn(
                name: "UnpaidDays",
                table: "EmployeeLeaves");

            migrationBuilder.InsertData(
                table: "LeavePolicies",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DaysPerMonth", "IsActive", "IsDeleted", "LeaveType", "MaximumDaysPerYear", "ModifiedBy", "ModifiedOn" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.5m, true, false, 1, 30, null, null },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1.25m, true, false, 2, 15, null, null }
                });
        }
    }
}
