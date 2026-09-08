using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace exam_system.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizStartAndEndDatesWithConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddCheckConstraint(
                name: "CK_Quiz_EndDate_After_StartDate",
                table: "Quizzes",
                sql: "[EndDate] > [StartDate]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Quiz_EndDate_After_StartDate",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Quizzes");
        }
    }
}
