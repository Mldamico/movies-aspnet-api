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
        modelBuilder.Entity<MoviesCinemas>()
            .HasKey(x => new {x.MovieId, x.CinemaId});
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MoviesGenres> MoviesGenres { get; set; }
    public DbSet<MoviesActors> MoviesActors { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<MoviesCinemas> MoviesCinemas { get; set; }
}