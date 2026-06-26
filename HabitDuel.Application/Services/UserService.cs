using Microsoft.EntityFrameworkCore;
using HabitDuel.Application.DTOs;
using HabitDuel.Application.Interfaces;
using HabitDuel.Domain.Entities;
using HabitDuel.Infrastructure.Persistence;

namespace HabitDuel.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterUserAsync(RegisterUserDto dto)
    {
        // Gunakan FirstOrDefaultAsync untuk operasi asinkron yang benar
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null) return false;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            // PERINGATAN: Sangat tidak aman menyimpan password plain text.
            // Gunakan library seperti BCrypt.Net untuk menghash password sebelum disimpan.
            PasswordHash = dto.Password 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // PERBAIKAN: Menambahkan '?' pada UserDto? agar sesuai dengan Interface
    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        // Jika tidak ditemukan, return null adalah valid sekarang karena return type-nya adalah UserDto?
        if (user == null) return null;

        return new UserDto(user.Username, user.Email);
    }
}