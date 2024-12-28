using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Capstone.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSearchVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "AspNetUsers",
                type: "tsvector",
                nullable: true,
                computedColumnSql: "to_tsvector('english', coalesce(\"UserName\", '') || ' ' || coalesce(\"Email\", '') || ' ' || coalesce(\"FirstName\", '') || ' ' || coalesce(\"LastName\", ''))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SearchVector",
                table: "AspNetUsers",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SearchVector",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "AspNetUsers");
        }
    }
}
