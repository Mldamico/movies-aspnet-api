using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;
using Movies.Services;

namespace MoviesTest.UnitTest;

[TestClass]
public class MoviesControllerTests : TestBase
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
        var page1 = await controller.GetActors(new PaginationDto() {Page = 1, AmountPerPage = 2});
        var actorsPage1 = page1.Value;
        Assert.AreEqual(2, actorsPage1.Count);

        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page2 = await controller.GetActors(new PaginationDto() {Page = 2, AmountPerPage = 2});
        var actorsPage2 = page2.Value;
        Assert.AreEqual(1, actorsPage2.Count);

        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page3 = await controller.GetActors(new PaginationDto() {Page = 3, AmountPerPage = 2});
        var actorsPage3 = page3.Value;
        Assert.AreEqual(0, actorsPage3.Count);
    }

    [TestMethod]
    public async Task CreateActorWithoutPhoto_ShouldWork()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var actor = new ActorCreateDto() {Name = "Random", BirthDate = DateTime.Now};
        var mock = new Mock<IFileManager>();
        mock.Setup(x => x.SaveFile(null, null, null, null)).Returns(Task.FromResult("url"));

        var controller = new ActorsController(context, mapper, mock.Object);
        var response = await controller.CreateActor(actor);
        var result = response as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);

        var context2 = BuildContext(nameDb);
        var actors = await context2.Actors.ToListAsync();
        Assert.AreEqual(1, actors.Count);
        Assert.IsNull(actors[0].Photo);
        Assert.AreEqual(0, mock.Invocations.Count);

    }
}