using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Helpers;
using NetTopologySuite;

namespace MoviesTest;

public class TestBase
{
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
}