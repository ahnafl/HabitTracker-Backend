using HabitDuel.Application.DTOs;
using HabitDuel.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitDuel.API.Controllers;

[ApiController]
// FIX: Diubah menjadi 'api/user' agar sesuai dengan pemanggilan frontend Anda
[Route("api/user")] 
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    // Constructor untuk Dependency Injection
    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    // 1. Endpoint Registrasi
    // URL: POST /api/user/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _userService.RegisterUserAsync(dto);
        if (!result) 
            return BadRequest(new { message = "Username atau Email sudah terdaftar." });

        return Ok(new { message = "Registrasi berhasil!" });
    }

    // 2. Endpoint Login
    // URL: POST /api/user/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        
        if (token == null) 
            return Unauthorized(new { message = "Username atau password salah." });

        return Ok(new { token = token });
    }

    // 3. Endpoint Profil (Membutuhkan Token JWT)
    // URL: GET /api/user/profile
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Mencoba mendapatkan username dari klaim token
        var username = User.FindFirst(ClaimTypes.Name)?.Value 
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(username)) 
            return Unauthorized(new { message = "Token tidak valid atau user tidak terdeteksi." });

        var user = await _userService.GetByUsernameAsync(username);
        
        if (user == null) 
            return NotFound(new { message = "User tidak ditemukan." });

        return Ok(user);
    }
}