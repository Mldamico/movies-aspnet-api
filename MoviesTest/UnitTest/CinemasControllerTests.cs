using Microsoft.AspNetCore.Mvc;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesTest.UnitTest;

[TestClass]
public class CinemasControllerTests: TestBase
{

    [TestMethod]
    public async Task GetCinemas_With3Cinemas_ShouldReturnCorrectValue()
    {
        var dbName = Guid.NewGuid().ToString();
        var context = BuildContext(dbName);
        var mapper = ConfigurateAutoMapper();

        var cinemas = new List<Cinema>()
        {
            new Cinema { Name = "Devoto Shopping"},
            new Cinema { Name = "Unicenter Shopping"},
            new Cinema { Name = "Abasto Shopping"},
        };
        context.AddRange(cinemas);
        await context.SaveChangesAsync();

        var context2 = BuildContext(dbName);
        var controller = new CinemasController(context2, mapper, null);

        var response = await controller.GetCinemas();
        Assert.AreEqual(3, response.Value.Count);
    }

    [TestMethod]
    public async Task GetCinemaById_WithIncorrectValue_ShouldReturnError()
    {
        var dbName = Guid.NewGuid().ToString();
        var context = BuildContext(dbName);
        var mapper = ConfigurateAutoMapper();
        var controller = new CinemasController(context, mapper, null);
        var response = await controller.GetCinemaById(1);
        var result = response.Result as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task GetCinemaById_WithCorrectID_ShouldReturnCorrectCinema()
    {
        var dbName = Guid.NewGuid().ToString();
        var context = BuildContext(dbName);
        var mapper = ConfigurateAutoMapper();
        var cinema = new Cinema() { Name = "Devoto Shopping"};
        context.Add(cinema);
        await context.SaveChangesAsync();
        var context2 = BuildContext(dbName);
        var controller = new CinemasController(context2,mapper, null);
        var response = await controller.GetCinemaById(1);
        Assert.AreEqual("Devoto Shopping", response.Value.Name);
    }

    [TestMethod]
    public async Task GetCinemas_with5KMFilter()
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var dbName = Guid.NewGuid().ToString();
        var context = BuildContext(dbName);
        var cinemas = new List<Cinema>()
        {
            new Cinema{ Name = "Cine Random", Address = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))}
        };
        context.AddRange(cinemas);
        await context.SaveChangesAsync();
        var filter = new CinemaDistanceFilterDto()
        {
            DistanceKm = 5, Longitude = -69.9388777, Latitude = 18.4839233
        };

      
        var mapper = ConfigurateAutoMapper();
        var context2 = BuildContext(dbName);
        var controller = new CinemasController(context2, mapper, geometryFactory);
        var response = await controller.Near(filter);
        var value = response.Value;
        Assert.AreEqual(1, value.Count);
        
    }

}