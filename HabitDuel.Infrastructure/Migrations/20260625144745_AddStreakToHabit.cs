using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitDuel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakToHabit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Streak",
                table: "Habits",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Streak",
                table: "Habits");
        }
    }
}
