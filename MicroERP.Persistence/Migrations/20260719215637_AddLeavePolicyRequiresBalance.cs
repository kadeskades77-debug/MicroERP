using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeavePolicyRequiresBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresBalance",
                table: "LeavePolicies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "AllowNegativeBalance", "RequiresBalance" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -3,
                column: "RequiresBalance",
                value: true);

            migrationBuilder.UpdateData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -2,
                column: "RequiresBalance",
                value: true);

            migrationBuilder.UpdateData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -1,
                column: "RequiresBalance",
                value: true);

            migrationBuilder.InsertData(
                table: "LeavePolicies",
                columns: new[] { "Id", "AllowNegativeBalance", "CreatedBy", "CreatedOn", "DaysPerMonth", "IsActive", "IsDeleted", "LeaveType", "MaxNegativeDays", "MaximumDaysPerYear", "ModifiedBy", "ModifiedOn", "RequiresBalance" },
                values: new object[,]
                {
                    { -6, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 6, 0, 5, null, null, false },
                    { -5, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 5, 0, 60, null, null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DropColumn(
                name: "RequiresBalance",
                table: "LeavePolicies");

            migrationBuilder.UpdateData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -4,
                column: "AllowNegativeBalance",
                value: true);
        }
    }
}
