using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace exam_system.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizDurationMinutesPositiveConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Quiz_DurationMinutes_Positive",
                table: "Quizzes",
                sql: "[DurationMinutes] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Quiz_DurationMinutes_Positive",
                table: "Quizzes");
        }
    }
}
