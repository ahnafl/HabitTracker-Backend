namespace HabitDuel.Domain.Entities;

public class Duel
{
    public int Id { get; set; }
    public string ChallengerId { get; set; } = string.Empty;
public string OpponentId { get; set; } = string.Empty;
    public string HabitName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Active, Finished

    public User? Challenger { get; set; }
    public User? Opponent { get; set; }
}