using HabitDuel.Application.DTOs;
using HabitDuel.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitDuel.API.Controllers;

[Authorize] 
[ApiController]
[Route("api/habits")]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;

    public HabitsController(IHabitService habitService) 
        => _habitService = habitService;

    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<IActionResult> GetMyHabits()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var habits = await _habitService.GetMyHabitsByUserIdAsync(userId);
        return Ok(habits);
    }

    // Menggunakan route absolut untuk menghindari konflik dengan {id}
    [HttpGet("~/api/habits/stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        
        var stats = await _habitService.GetHabitStatsAsync(userId);
        return Ok(stats);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHabitDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var habit = await _habitService.CreateHabitAsync(userId, dto);
        return Ok(habit);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHabitDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _habitService.UpdateHabitAsync(id, userId, dto);
        return success ? Ok(new { message = "Habit berhasil diperbarui" }) : NotFound();
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _habitService.CompleteHabitAsync(id, userId);
        
        return success 
            ? Ok(new { message = "Habit selesai untuk hari ini!" }) 
            : BadRequest(new { message = "Gagal: Habit tidak ditemukan atau sudah diselesaikan hari ini." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _habitService.DeleteHabitAsync(id, userId);
        return success ? Ok(new { message = "Habit berhasil dihapus" }) : NotFound();
    }
}