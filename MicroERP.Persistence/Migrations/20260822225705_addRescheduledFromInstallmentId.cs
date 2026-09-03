using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addRescheduledFromInstallmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RescheduledFromInstallmentId",
                table: "EmployeeLoanInstallments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLoanInstallments_RescheduledFromInstallmentId",
                table: "EmployeeLoanInstallments",
                column: "RescheduledFromInstallmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLoanInstallments_EmployeeLoanInstallments_RescheduledFromInstallmentId",
                table: "EmployeeLoanInstallments",
                column: "RescheduledFromInstallmentId",
                principalTable: "EmployeeLoanInstallments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLoanInstallments_EmployeeLoanInstallments_RescheduledFromInstallmentId",
                table: "EmployeeLoanInstallments");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLoanInstallments_RescheduledFromInstallmentId",
                table: "EmployeeLoanInstallments");

            migrationBuilder.DropColumn(
                name: "RescheduledFromInstallmentId",
                table: "EmployeeLoanInstallments");
        }
    }
}
