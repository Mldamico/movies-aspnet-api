using Microsoft.AspNetCore.Http;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;

namespace MoviesTest.UnitTest;

[TestClass]
public class MoviesControllerTests: TestBase
{

    [TestMethod]
    public async Task GetActorsPagination()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Actors.Add(new Actor() {Name = "Actor 1"});
        context.Actors.Add(new Actor() {Name = "Actor 2"});
        context.Actors.Add(new Actor() {Name = "Actor 3"});

        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new ActorsController(context2, mapper, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page1 = await controller.GetActors(new PaginationDto(){ Page = 1, AmountPerPage = 2});
        var actorsPage1 = page1.Value;
        Assert.AreEqual(2, actorsPage1.Count);
        
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page2 = await controller.GetActors(new PaginationDto(){Page = 2, AmountPerPage = 2});
        var actorsPage2 = page2.Value;
        Assert.AreEqual(1, actorsPage2.Count);
        
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page3 = await controller.GetActors(new PaginationDto(){Page = 3, AmountPerPage = 2});
        var actorsPage3 = page3.Value;
        Assert.AreEqual(0, actorsPage3.Count);
    }
}