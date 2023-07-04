using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Services;

namespace Movies;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson();
        services.AddTransient<IFileManager, AzureFileManager>();
        // services.AddTransient<IFileManager, LocalFileManager>();
        services.AddHttpContextAccessor(); // For local resources
        services.AddAutoMapper(typeof(Startup));
        // services.AddEndpointsApiExplorer();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
