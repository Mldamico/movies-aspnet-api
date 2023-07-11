using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.Entities;

namespace Movies.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoviesActors>()
            .HasKey(x => new {x.ActorId, x.MovieId});
        modelBuilder.Entity<MoviesGenres>()
            .HasKey(x => new {x.GenreId, x.MovieId});
        modelBuilder.Entity<MoviesCinemas>()
            .HasKey(x => new {x.MovieId, x.CinemaId});

        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminRoleId = "bcd2b5b5-6335-4639-8f99-b0f9126f92ed";
        var userAdminId = "d7a2855a-b5f3-4b1d-9df0-b05805bea60c";

        var adminRole = new IdentityRole()
        {
            Id = adminRoleId,
            Name = "Admin",
            NormalizedName = "Admin"
        };

        var passwordHasher = new PasswordHasher<IdentityUser>();
        var username = "adminUser";
        var email = "admin@gmail.com";

            var userAdmin = new IdentityUser()
        {
            Id = userAdminId,
            UserName = username,
            NormalizedUserName = username,
            Email = email,
            NormalizedEmail = email,
            PasswordHash = passwordHasher.HashPassword(null,"P$ssw0rd")
        };
            
            modelBuilder.Entity<IdentityUser>()
                .HasData(userAdmin);
            
            modelBuilder.Entity<IdentityRole>()
                .HasData(adminRole);
            
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = userAdminId,
                    ClaimValue = "Admin"
                });
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MoviesGenres> MoviesGenres { get; set; }
    public DbSet<MoviesActors> MoviesActors { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<MoviesCinemas> MoviesCinemas { get; set; }
}