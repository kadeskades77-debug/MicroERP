using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "EmployeeSpecialLeaves");

            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "EmployeeLeaves");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "EmployeeSpecialLeaves",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LeaveAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmployeeLeaveId = table.Column<int>(type: "int", nullable: true),
                    EmployeeSpecialLeaveId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAttachments_EmployeeLeaves_EmployeeLeaveId",
                        column: x => x.EmployeeLeaveId,
                        principalTable: "EmployeeLeaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveAttachments_EmployeeSpecialLeaves_EmployeeSpecialLeaveId",
                        column: x => x.EmployeeSpecialLeaveId,
                        principalTable: "EmployeeSpecialLeaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_ApprovedByUserId",
                table: "EmployeeLeaves",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_RejectedByUserId",
                table: "EmployeeLeaves",
                column: "RejectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAttachments_EmployeeLeaveId",
                table: "LeaveAttachments",
                column: "EmployeeLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAttachments_EmployeeSpecialLeaveId",
                table: "LeaveAttachments",
                column: "EmployeeSpecialLeaveId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeaves_AspNetUsers_ApprovedByUserId",
                table: "EmployeeLeaves",
                column: "ApprovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeaves_AspNetUsers_RejectedByUserId",
                table: "EmployeeLeaves",
                column: "RejectedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeaves_AspNetUsers_ApprovedByUserId",
                table: "EmployeeLeaves");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeaves_AspNetUsers_RejectedByUserId",
                table: "EmployeeLeaves");

            migrationBuilder.DropTable(
                name: "LeaveAttachments");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeaves_ApprovedByUserId",
                table: "EmployeeLeaves");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeaves_RejectedByUserId",
                table: "EmployeeLeaves");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "EmployeeSpecialLeaves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "EmployeeSpecialLeaves",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "EmployeeLeaves",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
