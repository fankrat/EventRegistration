using Microsoft.EntityFrameworkCore;
using EventRegistration.Models;

namespace EventRegistration.Data;

public class AppDbContext : DbContext
{
    // Конструктор, который передаёт настройки подключения (строку подключения к SQLite)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Таблицы в базе данных
    public DbSet<Event> Events { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Registration> Registrations { get; set; }
}