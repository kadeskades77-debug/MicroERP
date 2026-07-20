using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentPathToEmployeeLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "EmployeeLeaves",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "EmployeeLeaves");
        }
    }
}
