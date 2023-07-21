using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;

namespace MoviesTest.UnitTest;

[TestClass]
public class ReviewsControllerTests : TestBase
{


    [TestMethod]
    public async Task CreateReview_IfUserWantToPostTwoReviewsInTheSameMovie_ShouldThrowAnError()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        CreateMovies(nameDb);
        var movieId = context.Movies.Select(x => x.Id).First();
        var firstReview = new Review() {MovieId = movieId, UserId = userDefaultId, Score = 10};
        context.Add(firstReview);
        await context.SaveChangesAsync();
        var context2 = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var controller = new ReviewController(context2, mapper);
        controller.ControllerContext = BuildControllerContext();
        var reviewCreateDto = new ReviewCreateDto() { Score = 10};
        var response = await controller.CreateReview(movieId, reviewCreateDto);
        var value = response as IStatusCodeActionResult;
        Assert.AreEqual(400, value.StatusCode.Value);
    }
    
    [TestMethod]
    public async Task CreateReview_WrittingOneReview_ShouldWorkSuccessfully()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        CreateMovies(nameDb);
        var movieId = context.Movies.Select(x => x.Id).First();
        
        var context2 = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var controller = new ReviewController(context2, mapper);
        controller.ControllerContext = BuildControllerContext();
        var reviewCreateDto = new ReviewCreateDto() { Score = 10};
        var response = await controller.CreateReview(movieId, reviewCreateDto);
        var value = response as NoContentResult;
        Assert.IsNotNull(value);
        var context3 = BuildContext(nameDb);
        var reviewDb = context3.Reviews.First();
        Assert.AreEqual(userDefaultId, reviewDb.UserId);
    }
    
    protected void CreateMovies(string nameDb)
    {
        var context = BuildContext(nameDb);
        context.Movies.Add(new Movie() {Title = "Test Movie"});

        context.SaveChanges();
    }
    
}