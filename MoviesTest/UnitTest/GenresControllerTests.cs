using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;

namespace MoviesTest.UnitTest;

[TestClass]
public class GenresControllerTests : TestBase
{
    [TestMethod]
    public async Task GetAllGenres()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Genres.Add(new Genre() {Name = "Genre 1"});
        context.Genres.Add(new Genre() {Name = "Genre 2"});
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);

        var controller = new GenresController(context2, mapper);
        var response = await controller.GetGenres();

        var genres = response.Value;

        Assert.AreEqual(2, genres.Count);
    }

    [TestMethod]
    public async Task GetGenreById_WithInvalidId_ShouldReturnError()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new GenresController(context, mapper);
        var response = await controller.GetGenreById(1);

        var result = response.Result as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }
    
    [TestMethod]
    public async Task GetGenreById_WithValidId_ShouldReturnGenre()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        
        context.Genres.Add(new Genre() {Name = "Genre 1"});
        context.Genres.Add(new Genre() {Name = "Genre 2"});
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);

        var controller = new GenresController(context2, mapper);
        var response = await controller.GetGenreById(1);

        var result = response.Value;
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public async Task CreateGenre()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var newGenre = new GenreCreateDto() { Name = "new genre"};
        var controller = new GenresController(context, mapper);
        var response = await controller.CreateGenre(newGenre);
        var result = response as CreatedAtRouteResult;
        Assert.IsNotNull(result);
        
        var context2 = BuildContext(nameDb);
        var amount = await context2.Genres.CountAsync();
        Assert.AreEqual(1, amount);
    }

    [TestMethod]
    public async Task UpdateGenre()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Genres.Add(new Genre() {Name = "Genre 1"});

        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new GenresController(context2, mapper);
        var genreCreateDto = new GenreCreateDto() {Name = "New Name"};
        var id = 1;
        var response = await controller.UpdateGenre(id, genreCreateDto);

        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exist = await context3.Genres.AnyAsync(x => x.Name == "New Name");
        Assert.IsTrue(exist);
    }

    [TestMethod]
    public async Task DeleteGenre_TryingRemoveWithInvalidID_shouldReturnError()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new GenresController(context, mapper);
        var response = await controller.DeleteGenre(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteGenre_WithValidId_ShouldWork()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        
        context.Genres.Add(new Genre() {Name = "Genre 1"});

        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new GenresController(context2, mapper);
        var response = await controller.DeleteGenre(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exist = await context3.Genres.AnyAsync();
        Assert.IsFalse(exist);

    }
}