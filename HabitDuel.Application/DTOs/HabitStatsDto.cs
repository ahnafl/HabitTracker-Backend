namespace HabitDuel.Application.DTOs;

public class HabitStatsDto
{
    public int CompletedLast7Days { get; set; }
    public int TotalHabits { get; set; }
    public double CompletionRate { get; set; } // Persentase keberhasilan
}