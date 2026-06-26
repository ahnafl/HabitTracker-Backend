using BCrypt.Net;
using HabitDuel.Application.DTOs;
using HabitDuel.Application.Interfaces;
using HabitDuel.Domain.Entities;
using HabitDuel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitDuel.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User 
        { 
            Username = dto.Username, 
            Email = dto.Email, 
            PasswordHash = passwordHash 
        };
        
        _context.Users.Add(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username);
            
        // Pengecekan null dilakukan di sini
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        // Sekarang parameter pertama sudah string, cocok dengan IJwtService
        return _jwtService.GenerateToken(user.Id, user.Username);
    }
}