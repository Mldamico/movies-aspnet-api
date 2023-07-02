using Microsoft.EntityFrameworkCore;
using Movies.Entities;

namespace Movies.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options):base(options)
    {
        
    }
    
    public DbSet<Genre> Genres { get; set; }
}