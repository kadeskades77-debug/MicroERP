using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionAssignmentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSingle",
                table: "Positions");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentType",
                table: "Positions",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentType",
                table: "Positions");

            migrationBuilder.AddColumn<bool>(
                name: "IsSingle",
                table: "Positions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
