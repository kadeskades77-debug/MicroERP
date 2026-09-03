using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BankAccountNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankAccountSnapshot",
                table: "Payrolls",
                newName: "IBAN");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Payrolls");

            migrationBuilder.RenameColumn(
                name: "IBAN",
                table: "Payrolls",
                newName: "BankAccountSnapshot");
        }
    }
}
