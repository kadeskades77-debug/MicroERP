using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttendanceTransactionDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceTransactions_AttendanceLogId",
                table: "AttendanceTransactions");

            migrationBuilder.AlterColumn<byte>(
                name: "Type",
                table: "AttendanceTransactions",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceLogId",
                table: "AttendanceTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceDeviceId",
                table: "AttendanceTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "AttendanceTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTransactions_AttendanceLogId",
                table: "AttendanceTransactions",
                column: "AttendanceLogId",
                unique: true,
                filter: "[AttendanceLogId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceTransactions_AttendanceLogId",
                table: "AttendanceTransactions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "AttendanceTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "AttendanceTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceLogId",
                table: "AttendanceTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceDeviceId",
                table: "AttendanceTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTransactions_AttendanceLogId",
                table: "AttendanceTransactions",
                column: "AttendanceLogId",
                unique: true);
        }
    }
}
