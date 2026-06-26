using HabitDuel.Application.Interfaces;
using HabitDuel.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HabitDuel.Application; // Namespace-nya harus ini

public static class DependencyInjection // Pastikan 'public'
{
    public static IServiceCollection AddApplication(this IServiceCollection services) // Pastikan 'public'
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IHabitService, HabitService>(); // Jangan lupa daftarkan ini juga
        return services;
    }
}