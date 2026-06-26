using HabitDuel.Application.DTOs;
using HabitDuel.Domain.Entities;

namespace HabitDuel.Application.Interfaces;

public interface IUserService
{
    Task<bool> RegisterUserAsync(RegisterUserDto dto);
    Task<UserDto?> GetByUsernameAsync(string username);
}
