using HabitDuel.Application.DTOs;
using HabitDuel.Application.Interfaces;
using HabitDuel.Domain.Entities;
using HabitDuel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitDuel.Application.Services;

public class HabitService : IHabitService
{
    private readonly AppDbContext _context;

    public HabitService(AppDbContext context) => _context = context;

    // --- CRUD METHODS ---

    public async Task<IEnumerable<Habit>> GetMyHabitsByUserIdAsync(string userId)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId)
            .ToListAsync();
    }

    public async Task<Habit> CreateHabitAsync(string userId, CreateHabitDto dto)
    {
        var habit = new Habit
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            TargetDate = dto.TargetDate.HasValue ? DateOnly.FromDateTime(dto.TargetDate.Value) : null,
            CreatedAt = DateTime.UtcNow,
            Streak = 0,
            IsCompleted = false
        };

        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();
        return habit;
    }

    public async Task<bool> UpdateHabitAsync(int habitId, string userId, UpdateHabitDto dto)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
        
        if (habit == null) return false;

        habit.Title = dto.Title;
        habit.Description = dto.Description;
        habit.TargetDate = dto.TargetDate.HasValue ? DateOnly.FromDateTime(dto.TargetDate.Value) : null;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHabitAsync(int habitId, string userId)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
        
        if (habit == null) return false;

        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync();
        return true;
    }

    // --- ACTION METHODS ---

    public async Task<bool> CompleteHabitAsync(int habitId, string userId)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
        
        if (habit == null) return false;

        // 1. Cek apakah sudah diselesaikan hari ini (untuk membatasi 1x per hari)
        var today = DateTime.UtcNow.Date;
        var alreadyDone = await _context.HabitLogs
            .AnyAsync(l => l.HabitId == habitId && l.CompletedAt.Date == today);

        if (alreadyDone) return false; // Berhenti jika sudah diklik hari ini

        // 2. Tambah Log
        var log = new HabitLog { HabitId = habitId, CompletedAt = DateTime.UtcNow };
        _context.HabitLogs.Add(log);
        
        // 3. Tambah Streak secara instan
        habit.Streak += 1;
        habit.IsCompleted = true;
        
        await _context.SaveChangesAsync();
        return true;
    }

    // --- STATS METHODS ---

    public async Task<HabitStatsDto> GetHabitStatsAsync(string userId)
    {
        var habits = await _context.Habits
            .Where(h => h.UserId == userId)
            .ToListAsync();

        var habitIds = habits.Select(h => h.Id).ToList();
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var completedCount = await _context.HabitLogs
            .Where(l => habitIds.Contains(l.HabitId) && l.CompletedAt >= sevenDaysAgo)
            .CountAsync();

        return new HabitStatsDto
        {
            CompletedLast7Days = completedCount,
            TotalHabits = habits.Count,
            CompletionRate = habits.Count > 0 ? (double)completedCount / (habits.Count * 7) * 100 : 0
        };
    }

    public async Task<int> GetCurrentStreakAsync(int habitId)
    {
        // Method ini tetap disimpan jika sewaktu-waktu Anda ingin melakukan sinkronisasi ulang
        var logDates = await _context.HabitLogs
            .Where(l => l.HabitId == habitId)
            .Select(l => l.CompletedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync();

        if (!logDates.Any()) return 0;

        int streak = 0;
        var checkDate = DateTime.UtcNow.Date;

        if (!logDates.Contains(checkDate)) checkDate = checkDate.AddDays(-1);
        if (!logDates.Contains(checkDate)) return 0;

        while (logDates.Contains(checkDate))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }
        return streak;
    }
}