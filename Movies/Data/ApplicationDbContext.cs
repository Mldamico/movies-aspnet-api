using Microsoft.EntityFrameworkCore;
using Movies.Entities;

namespace Movies.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options):base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoviesActors>()
            .HasKey(x => new { x.ActorId, x.MovieId });
        modelBuilder.Entity<MoviesGenres>()
            .HasKey(x => new {  x.GenreId, x.MovieId });
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MoviesGenres> MoviesGenres { get; set; }
    public DbSet<MoviesActors> MoviesActors { get; set; }
}