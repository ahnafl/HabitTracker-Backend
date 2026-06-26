using HabitDuel.Application.DTOs;
using HabitDuel.Domain.Entities;

namespace HabitDuel.Application.Interfaces;

public interface IHabitService
{
    Task<IEnumerable<Habit>> GetMyHabitsByUserIdAsync(string userId);
    Task<Habit> CreateHabitAsync(string userId, CreateHabitDto dto);
    Task<bool> UpdateHabitAsync(int habitId, string userId, UpdateHabitDto dto);
    Task<bool> DeleteHabitAsync(int habitId, string userId);
    
    // Perbaikan: Tambahkan userId di sini
    Task<bool> CompleteHabitAsync(int habitId, string userId);
    
    // Perbaikan: Samakan nama dengan yang ada di HabitService.cs
    Task<int> GetCurrentStreakAsync(int habitId); 
    
    Task<HabitStatsDto> GetHabitStatsAsync(string userId);
}