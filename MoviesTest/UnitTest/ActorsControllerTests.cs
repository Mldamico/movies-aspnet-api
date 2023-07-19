using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;
using Movies.Services;

namespace MoviesTest.UnitTest;

[TestClass]
public class ActorsControllerTests : TestBase
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
    
    [TestMethod]
    public async Task CreateActorWithPhoto_ShouldWork()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var content = Encoding.UTF8.GetBytes("Photo Image");
        var file = new FormFile(new MemoryStream(content),0, content.Length, "Data","photo.jpg");
        file.Headers = new HeaderDictionary();
        file.ContentType = "image/jpg";
        
        var actor = new ActorCreateDto() {Name = "Random", BirthDate = DateTime.Now, Photo = file};
        var mock = new Mock<IFileManager>();
        mock.Setup(x => x.SaveFile(content, ".jpg", "actors", file.ContentType)).Returns(Task.FromResult("url"));

        var controller = new ActorsController(context, mapper, mock.Object);
        var response = await controller.CreateActor(actor);
        var result = response as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);

        var context2 = BuildContext(nameDb);
        var actors = await context2.Actors.ToListAsync();
        Assert.AreEqual(1, actors.Count);
        Assert.AreEqual("url",actors[0].Photo);
        
        Assert.AreEqual(1, mock.Invocations.Count);

    }

    [TestMethod]
    public async Task PatchActor_ShouldReturn404_IfActorDoesNotExists()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new ActorsController(context, mapper, null);
        var patchDoc = new JsonPatchDocument<ActorPatchDto>();
        var response = await controller.UpdateActorPatch(1, patchDoc);
        var result = response as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }
    
    [TestMethod]
    public async Task PatchActor_ShouldSuccessChangeOneField_IfOnlyOnFieldIsUpdated()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var birthDate = DateTime.Now;
        var actor = new Actor(){Name = "Test", BirthDate = birthDate};
        context.Add(actor);
        await context.SaveChangesAsync();
        var context2 = BuildContext(nameDb);
        var controller = new ActorsController(context2, mapper, null);
        var objectValidator = new Mock<IObjectModelValidator>();
        objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(), 
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<object>()));

        controller.ObjectValidator = objectValidator.Object;
        var patchDoc = new JsonPatchDocument<ActorPatchDto>();
        patchDoc.Operations.Add(new Operation<ActorPatchDto>("replace","/name", null, "New Value"));
        var response = await controller.UpdateActorPatch(1, patchDoc);
        var result = response as StatusCodeResult;
       
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var actorDb = await context3.Actors.FirstAsync();
        Assert.AreEqual("New Value", actorDb.Name);
        Assert.AreEqual(birthDate, actorDb.BirthDate);
    }

    [TestMethod]
    public async Task UpdateActor_WithPutMethod_ShouldWork()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Actors.Add(new Actor() {Name = "Actor 1"});

        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new ActorsController(context2, mapper, null);
        var actorCreateDto = new ActorCreateDto() {Name = "New Actor Name"};
        var response = await controller.UpdateActor(1, actorCreateDto);


        var result = response.Result as OkObjectResult;
        Assert.AreEqual(200, result.StatusCode);
        
        var context3 = BuildContext(nameDb);
        var exist = await context3.Actors.AnyAsync(x => x.Name == "New Actor Name");
        Assert.IsTrue(exist);
    }
    
    [TestMethod]
    public async Task DeleteActor_TryingRemoveWithInvalidID_shouldReturnError()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new ActorsController(context, mapper, null);
        var response = await controller.DeleteActor(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }
    
    [TestMethod]
    public async Task DeleteActor_WithCorrectID_ShouldRemoveActorSuccessfully()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var actor = new Actor() { Name = "Test Actor"};
        context.Actors.Add(actor);
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var actorController = new ActorsController(context2, mapper, null);
        var response = await actorController.DeleteActor(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exists = await context3.Actors.AnyAsync();
        Assert.IsFalse(exists);
    }
}