using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.Migrations
{
    /// <inheritdoc />
    public partial class OptionAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Forms_FormId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "FormId",
                table: "Answers",
                newName: "TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_FormId",
                table: "Answers",
                newName: "IX_Answers_TemplateId");

            migrationBuilder.AddColumn<int>(
                name: "NumericResponse",
                table: "Answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StringResponse",
                table: "Answers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OptionAnswers",
                columns: table => new
                {
                    OptionAnswerId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    AnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionAnswers", x => x.OptionAnswerId);
                    table.ForeignKey(
                        name: "FK_OptionAnswers_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "AnswerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptionAnswers_QuestionOptions_QuestionOptionId",
                        column: x => x.QuestionOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "QuestionOptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionAnswers_AnswerId",
                table: "OptionAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionAnswers_QuestionOptionId",
                table: "OptionAnswers",
                column: "QuestionOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Templates_TemplateId",
                table: "Answers",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Templates_TemplateId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "OptionAnswers");

            migrationBuilder.DropColumn(
                name: "NumericResponse",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "StringResponse",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "Answers",
                newName: "FormId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_TemplateId",
                table: "Answers",
                newName: "IX_Answers_FormId");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Answers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    FormId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    FilledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_Forms_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Forms_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forms_TemplateId",
                table: "Forms",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UserId",
                table: "Forms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Forms_FormId",
                table: "Answers",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "FormId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
