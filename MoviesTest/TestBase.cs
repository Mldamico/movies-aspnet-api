using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Helpers;
using NetTopologySuite;

namespace MoviesTest;

public class TestBase
{
    protected string userDefaultId = "ABCD-123456";
    protected string userDefaultEmail = "testing@gmail.com";
    protected ApplicationDbContext BuildContext(string nameDb)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameDb).Options;

        var dbContext = new ApplicationDbContext(options);

        return dbContext;

    }

    protected IMapper ConfigurateAutoMapper()
    {
        var config = new MapperConfiguration(options =>
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid:4326);
            options.AddProfile(new AutoMapperProfiles(geometryFactory));
        });

        return config.CreateMapper();
    }

    protected ControllerContext BuildControllerContext()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, userDefaultEmail),
            new Claim(ClaimTypes.Email, userDefaultEmail),
            new Claim(ClaimTypes.NameIdentifier, userDefaultId),
        }));

        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() {User = user}
        };
    }
}