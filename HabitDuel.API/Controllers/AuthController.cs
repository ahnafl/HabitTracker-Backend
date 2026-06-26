using HabitDuel.Application.DTOs;
using HabitDuel.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitDuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto);
        
        if (!success) 
            return BadRequest(new { message = "Gagal mendaftar. Username mungkin sudah terpakai." });
        
        return Ok(new { message = "User berhasil didaftarkan!" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        
        if (token == null) 
            return Unauthorized(new { message = "Username atau password salah." });
        
        return Ok(new { Token = token });
    }
}