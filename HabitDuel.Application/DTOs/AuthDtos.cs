using System.ComponentModel.DataAnnotations;

namespace HabitDuel.Application.DTOs;

public record RegisterDto(
    [Required(ErrorMessage = "Username wajib diisi")]
    string Username, 

    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    string Email, 

    [Required(ErrorMessage = "Password wajib diisi")]
    [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
    string Password
);

public record LoginDto(
    [Required(ErrorMessage = "Username wajib diisi")]
    string Username, 

    [Required(ErrorMessage = "Password wajib diisi")]
    string Password
);

// Tambahan: DTO untuk respon setelah Login (agar rapi)
public record AuthResponseDto(
    string Token,
    string Username
);