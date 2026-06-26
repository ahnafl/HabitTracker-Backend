namespace HabitDuel.Application.DTOs;

public class HabitDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int Streak { get; set; }
    public DateOnly? TargetDate { get; set; } 
    public DateTime CreatedAt { get; set; }

    // Constructor utama
    public HabitDto(int id, string title, string? description, bool isCompleted, int streak, DateOnly? targetDate, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        IsCompleted = isCompleted;
        Streak = streak;
        TargetDate = targetDate;
        CreatedAt = createdAt;
    }
}