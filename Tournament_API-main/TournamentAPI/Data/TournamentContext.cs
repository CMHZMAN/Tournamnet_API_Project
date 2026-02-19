using Microsoft.EntityFrameworkCore;
using TournamentAPI.Models;

namespace TournamentAPI.Data;

public class TournamentContext : DbContext
{
    public TournamentContext(DbContextOptions<TournamentContext> options) : base(options)
    {
    }

    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Tournament
        modelBuilder.Entity<Tournament>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Tournament>()
            .Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Tournament>()
            .Property(t => t.Description)
            .HasMaxLength(500);

        // Configure Game
        modelBuilder.Entity<Game>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<Game>()
            .Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(100);

        // Configure foreign key and cascade delete
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Tournament)
            .WithMany(t => t.Games)
            .HasForeignKey(g => g.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
