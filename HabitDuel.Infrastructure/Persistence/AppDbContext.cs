using HabitDuel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitDuel.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<Duel> Duels { get; set; }
    public DbSet<HabitLog> HabitLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Konfigurasi User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
        });

        // 2. Konfigurasi Habit
        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            
            // FIX: Memastikan kolom TargetDate dipetakan sebagai 'date' di PostgreSQL
            entity.Property(e => e.TargetDate).HasColumnType("date");
            
            entity.HasOne(h => h.User)
                  .WithMany(u => u.Habits)
                  .HasForeignKey(h => h.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 3. Konfigurasi HabitLog
        modelBuilder.Entity<HabitLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Habit)
                  .WithMany() 
                  .HasForeignKey(e => e.HabitId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. Konfigurasi Duel
        modelBuilder.Entity<Duel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Challenger)
                  .WithMany()
                  .HasForeignKey(d => d.ChallengerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Opponent)
                  .WithMany()
                  .HasForeignKey(d => d.OpponentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}