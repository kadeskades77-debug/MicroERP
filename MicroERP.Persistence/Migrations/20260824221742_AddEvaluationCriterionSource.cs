using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationCriterionSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EvaluationPeriods_StartDate_EndDate",
                table: "EvaluationPeriods");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "EvaluationCriteria",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationPeriods_StartDate",
                table: "EvaluationPeriods",
                column: "StartDate",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EvaluationPeriods_StartDate",
                table: "EvaluationPeriods");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "EvaluationCriteria");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationPeriods_StartDate_EndDate",
                table: "EvaluationPeriods",
                columns: new[] { "StartDate", "EndDate" });
        }
    }
}
