namespace HabitDuel.Domain.Entities;

public class HabitLog
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    
    // Navigasi
    public Habit Habit { get; set; } = null!;
}