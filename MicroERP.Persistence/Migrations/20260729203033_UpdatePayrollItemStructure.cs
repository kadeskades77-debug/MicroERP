using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayrollItemStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAdjustments_SalaryComponents_SalaryComponentId",
                table: "PayrollAdjustments");

            migrationBuilder.AlterColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PayrollItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "PayrollItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "PayrollItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PayrollItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollAdjustments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");




            migrationBuilder.AddForeignKey(
      name: "FK_PayrollAdjustments_SalaryComponents_SalaryComponentId",
      table: "PayrollAdjustments",
      column: "SalaryComponentId",
      principalTable: "SalaryComponents",
      principalColumn: "Id",
      onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAdjustments_SalaryComponents_SalaryComponentId",
                table: "PayrollAdjustments");

      

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PayrollItems");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "PayrollItems");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "PayrollItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PayrollItems");


            migrationBuilder.AlterColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollAdjustments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAdjustments_SalaryComponents_SalaryComponentId",
                table: "PayrollAdjustments",
                column: "SalaryComponentId",
                principalTable: "SalaryComponents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
