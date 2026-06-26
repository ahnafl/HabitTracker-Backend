using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitDuel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToHabit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Habits",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Habits");
        }
    }
}
