using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeSpecialLeaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeSpecialLeaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RejectedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSpecialLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSpecialLeaves_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSpecialLeaves_AspNetUsers_RejectedByUserId",
                        column: x => x.RejectedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSpecialLeaves_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "LeavePolicies",
                columns: new[] { "Id", "AllowNegativeBalance", "CreatedBy", "CreatedOn", "DaysPerMonth", "IsActive", "IsDeleted", "LeaveType", "MaxNegativeDays", "MaximumDaysPerYear", "ModifiedBy", "ModifiedOn", "RequiresBalance" },
                values: new object[,]
                {
                    { -9, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 9, 0, 30, null, null, false },
                    { -8, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 8, 0, 3, null, null, false },
                    { -7, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, false, 7, 0, 7, null, null, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSpecialLeaves_ApprovedByUserId",
                table: "EmployeeSpecialLeaves",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSpecialLeaves_EmployeeId_Type_Status",
                table: "EmployeeSpecialLeaves",
                columns: new[] { "EmployeeId", "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSpecialLeaves_RejectedByUserId",
                table: "EmployeeSpecialLeaves",
                column: "RejectedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSpecialLeaves");

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "LeavePolicies",
                keyColumn: "Id",
                keyValue: -7);
        }
    }
}
