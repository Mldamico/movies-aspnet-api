using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Data;
using Movies.Helpers;
using Movies.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;

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
       
        services.AddTransient<IFileManager, AzureFileManager>();
        // services.AddTransient<IFileManager, LocalFileManager>();
        services.AddHttpContextAccessor(); // For local resources
        services.AddAutoMapper(typeof(Startup));
        // services.AddEndpointsApiExplorer();
        services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
        services.AddScoped<MovieExistAttribute>();
        services.AddSingleton(provider => new MapperConfiguration(config =>
        {
            var geometryFactory = provider.GetRequiredService<GeometryFactory>();
            config.AddProfile(new AutoMapperProfiles(geometryFactory));
        }).CreateMapper());
        services.AddControllers();
            // .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"), opt => opt.UseNetTopologySuite()));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddCors();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                });
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
        app.UseCors(opt =>
        {
            opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173");
        });
        app.UseAuthorization();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
