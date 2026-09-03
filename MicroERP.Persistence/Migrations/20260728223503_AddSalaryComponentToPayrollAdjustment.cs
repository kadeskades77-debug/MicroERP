using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryComponentToPayrollAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollAdjustments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollAdjustments_SalaryComponentId",
                table: "PayrollAdjustments",
                column: "SalaryComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAdjustments_SalaryComponents_SalaryComponentId",
                table: "PayrollAdjustments",
                column: "SalaryComponentId",
                principalTable: "SalaryComponents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAdjustments_SalaryComponents_SalaryComponentId",
                table: "PayrollAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_PayrollAdjustments_SalaryComponentId",
                table: "PayrollAdjustments");

            migrationBuilder.DropColumn(
                name: "SalaryComponentId",
                table: "PayrollAdjustments");
        }
    }
}
