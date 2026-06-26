namespace HabitDuel.Application.DTOs;

public class CreateHabitDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? TargetDate { get; set; }
}