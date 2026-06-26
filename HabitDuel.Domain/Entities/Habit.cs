using System.ComponentModel.DataAnnotations;

namespace HabitDuel.Domain.Entities;

public class Habit
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Judul habit wajib diisi.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;

    public int Streak { get; set; } = 0;

    // Menggunakan DateOnly agar lebih pas dengan input type="date"
    // Jika Anda menggunakan .NET versi lama, tetap gunakan DateTime?
    public DateOnly? TargetDate { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // UserId wajib ada di database (Relasi)
    [Required]
    public string UserId { get; set; } = string.Empty;

    // Navigation property (bisa diabaikan oleh JSON serializer)
    public User User { get; set; } = null!;
}