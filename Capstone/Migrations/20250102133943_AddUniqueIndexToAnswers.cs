using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_TemplateId",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_TemplateId_UserId",
                table: "Answers",
                columns: new[] { "TemplateId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_TemplateId_UserId",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_TemplateId",
                table: "Answers",
                column: "TemplateId");
        }
    }
}
