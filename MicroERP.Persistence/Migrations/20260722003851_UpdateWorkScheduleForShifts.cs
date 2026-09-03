using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkScheduleForShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceTransactions_WorkSchedules_WorkScheduleId",
                table: "AttendanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_IsDefault",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_Name",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTransactions_WorkScheduleId",
                table: "AttendanceTransactions");

            migrationBuilder.DropColumn(
                name: "WorkScheduleId",
                table: "AttendanceTransactions");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "WorkSchedules",
                newName: "FirstShiftStart");

            migrationBuilder.RenameColumn(
                name: "GracePeriodMinutes",
                table: "WorkSchedules",
                newName: "LateGraceMinutes");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "WorkSchedules",
                newName: "FirstShiftEnd");

            migrationBuilder.AlterColumn<int>(
                name: "MinimumWorkMinutes",
                table: "WorkSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 480);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                table: "WorkSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "EarlyLeaveGraceMinutes",
                table: "WorkSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SecondShiftEnd",
                table: "WorkSchedules",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SecondShiftStart",
                table: "WorkSchedules",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ShiftNumber",
                table: "AttendanceTransactions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_IsDefault",
                table: "WorkSchedules",
                column: "IsDefault",
                unique: true,
                filter: "[IsDefault] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_IsDefault",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "EarlyLeaveGraceMinutes",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "SecondShiftEnd",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "SecondShiftStart",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "ShiftNumber",
                table: "AttendanceTransactions");

            migrationBuilder.RenameColumn(
                name: "LateGraceMinutes",
                table: "WorkSchedules",
                newName: "GracePeriodMinutes");

            migrationBuilder.RenameColumn(
                name: "FirstShiftStart",
                table: "WorkSchedules",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "FirstShiftEnd",
                table: "WorkSchedules",
                newName: "EndTime");

            migrationBuilder.AlterColumn<int>(
                name: "MinimumWorkMinutes",
                table: "WorkSchedules",
                type: "int",
                nullable: false,
                defaultValue: 480,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                table: "WorkSchedules",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WorkScheduleId",
                table: "AttendanceTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_IsDefault",
                table: "WorkSchedules",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_Name",
                table: "WorkSchedules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTransactions_WorkScheduleId",
                table: "AttendanceTransactions",
                column: "WorkScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceTransactions_WorkSchedules_WorkScheduleId",
                table: "AttendanceTransactions",
                column: "WorkScheduleId",
                principalTable: "WorkSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
