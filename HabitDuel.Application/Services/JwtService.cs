using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HabitDuel.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HabitDuel.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(string userId, string username)
    {
        // 1. Validasi Keamanan: Pastikan key tersedia
        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("JWT Key belum diatur di appsettings.json!");

        // 2. Definisi Claims
        var claims = new[]
        {
            // Ini akan dibaca oleh User.FindFirst(ClaimTypes.NameIdentifier) di Controller
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        // 3. Konfigurasi Signing Key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Buat Token
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            // PENTING: Gunakan UtcNow agar tidak bentrok dengan zona waktu server
            expires: DateTime.UtcNow.AddDays(1), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}