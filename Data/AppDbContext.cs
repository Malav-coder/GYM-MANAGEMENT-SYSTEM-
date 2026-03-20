using GymManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "Admin",
            Phone = "0000000000",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Gender = "Other",
            Role = "Admin",
            IsFrozen = false,
            RegisteredAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<Plan>().HasData(
            new Plan { Id = 1, PlanName = "Monthly", DurationMonths = 1, Price = 800, IsActive = true },
            new Plan { Id = 2, PlanName = "Quarterly", DurationMonths = 3, Price = 2100, IsActive = true },
            new Plan { Id = 3, PlanName = "Half Yearly", DurationMonths = 6, Price = 3800, IsActive = true },
            new Plan { Id = 4, PlanName = "Yearly", DurationMonths = 12, Price = 6500, IsActive = true }
        );
    }
}
