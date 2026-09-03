using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollToEmployeeLoanInstallment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PayrollId",
                table: "EmployeeLoanInstallments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLoanInstallments_PayrollId",
                table: "EmployeeLoanInstallments",
                column: "PayrollId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLoanInstallments_Payrolls_PayrollId",
                table: "EmployeeLoanInstallments",
                column: "PayrollId",
                principalTable: "Payrolls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLoanInstallments_Payrolls_PayrollId",
                table: "EmployeeLoanInstallments");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLoanInstallments_PayrollId",
                table: "EmployeeLoanInstallments");

            migrationBuilder.DropColumn(
                name: "PayrollId",
                table: "EmployeeLoanInstallments");
        }
    }
}
