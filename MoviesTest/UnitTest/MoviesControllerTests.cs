using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;
using Movies.Services;

namespace MoviesTest.UnitTest;

[TestClass]
public class MoviesControllerTests: TestBase
{

    [TestMethod]
    public async Task GetAllMovies_ShowcasingList_ShouldReturnShowcasingMovies()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Movies.Add(new Movie() {Title = "Movie 1", Showcasing = true});
        context.Movies.Add(new Movie() {Title = "Movie 2", Showcasing = true});
        context.Movies.Add(new Movie() {Title = "Movie 3", Showcasing = false});
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);

        var controller = new MoviesController(context2, mapper, null, null);
        var response = await controller.GetMovies();

        var movies = response.Value;

        Assert.AreEqual(2, movies.Showcasing.Count);
    }

    [TestMethod]
    public async Task GetAllMovies_NextReleasingList_ShouldReturnNextReleasingMovies()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Movies.Add(new Movie() {Title = "Movie 1", DatePremiere = DateTime.Today.AddDays(1)});
        context.Movies.Add(new Movie() {Title = "Movie 2", DatePremiere = DateTime.Today.AddDays(1)});
        context.Movies.Add(new Movie() {Title = "Movie 3", DatePremiere = DateTime.Today.AddDays(-1)});
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);

        var controller = new MoviesController(context2, mapper, null, null);
        var response = await controller.GetMovies();

        var movies = response.Value;

        Assert.AreEqual(2, movies.NextRelease.Count);
    }
    
    
    [TestMethod]
    public async Task GetMovieById_WithInvalidId_ShouldReturnError()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new MoviesController(context, mapper, null, null);
        var response = await controller.GetMovieById(1);

        var result = response.Result as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task GetMovieById_WithValidId_ShouldReturnAMovie()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        
        context.Movies.Add(new Movie() {Title = "Movie 1"});
        await context.SaveChangesAsync();
        var context2 = BuildContext(nameDb);

        var moviesController = new MoviesController(context2, mapper, null, null);

        var response = await moviesController.GetMovieById(1);
        var result = response.Value;
        Assert.AreEqual("Movie 1", result.Title);
    }
    
    [TestMethod]
    public async Task CreateMovie_WithNoPoster_ShouldCreateMovieWithoutPoster()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var movie = new MovieCreateDto() { Title = "new movie"};
        var controller = new MoviesController(context, mapper, null, null);
        var response = await controller.CreateMovie(movie);
        var result = response as CreatedAtRouteResult;
        Assert.IsNotNull(result);
        
        var context2 = BuildContext(nameDb);
        var amount = await context2.Movies.CountAsync();
        Assert.AreEqual(1, amount);
    }

    [TestMethod]
    public async Task CreateMovie_WithPoster_ShouldCreateMovieWithPoster()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();
        var content = Encoding.UTF8.GetBytes("Photo Image");
        var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "photo.jpg");
        file.Headers = new HeaderDictionary();
        file.ContentType = "image/jpg";

        var movie = new MovieCreateDto() {Title = "Movie", Poster = file};
        var mock = new Mock<IFileManager>();
        mock.Setup(x => x.SaveFile(content, ".jpg", "actors", file.ContentType)).Returns(Task.FromResult("url"));

        var controller = new MoviesController(context, mapper, mock.Object, null);
        var response = await controller.CreateMovie(movie);
        var result = response as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);
    }

    [TestMethod]
    public async Task PatchMovie_ShouldReturn404_IfMovieDoesNotExists()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new MoviesController(context, mapper, null, null);
        var patchDocument = new JsonPatchDocument<MoviePatchDto>();
        var response = await controller.UpdateMoviePatch(1, patchDocument);
        var result = response.Result as NotFoundResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task FilterMovies_FilterByTitle_ShallReturnMoviesFilteredByTitles()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var titleMovie = "Movie 1";

        var filterDto = new FilterMoviesDto()
        {
            Title = titleMovie,
            RegisterPerPage = 5
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual(titleMovie, movies[0].Title);
    }

    [TestMethod]
    public async Task UpdateMovie_ShouldReturnTheMovie()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        context.Movies.Add(new Movie() {Title = "Movie 1"});

        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var controller = new MoviesController(context2, mapper, null, null);
        var movieCreateDto = new MovieCreateDto() {Title = "New Movie Name"};
        var response = await controller.UpdateMovie(1, movieCreateDto);

        
        var result = response as OkObjectResult;
        Assert.AreEqual(200, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exist = await context3.Movies.AnyAsync(x => x.Title == "New Movie Name");
        Assert.IsTrue(exist);
    }
    
    [TestMethod]
    public async Task DeleteMovie_TryingRemoveWithInvalidID_shouldReturnError()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var controller = new MoviesController(context, mapper, null, null);
        var response = await controller.DeleteMovie(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteMovie_WithCorrectID_ShouldRemoveMovieSuccessfully()
    {
        var nameDb = Guid.NewGuid().ToString();
        var context = BuildContext(nameDb);
        var mapper = ConfigurateAutoMapper();

        var movie = new Movie() { Title = "Test Movie"};
        context.Movies.Add(movie);
        await context.SaveChangesAsync();

        var context2 = BuildContext(nameDb);
        var moviesController = new MoviesController(context2, mapper, null, null);
        var response = await moviesController.DeleteMovie(1);
        var result = response as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = BuildContext(nameDb);
        var exists = await context3.Movies.AnyAsync();
        Assert.IsFalse(exists);
    }
    
    [TestMethod]
    public async Task FilterMovies_FilterByCinema_ShouldReturnMoviesWhichAreShowcasingOnCinema()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
       

        var filterDto = new FilterMoviesDto()
        {
            Showcasing =true,
            
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Movie 3 on cinema", movies[0].Title);
    }
    
    [TestMethod]
    public async Task FilterMovies_FilterByNextRelease_ShouldReturnMoviesWhichAreReleasingSoon()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
       

        var filterDto = new FilterMoviesDto()
        {
            NextRelease = true
            
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Movie 2 release tomorrow", movies[0].Title);
    }

    [TestMethod]
    public async Task FilterMovies_FilterByGenres_ShouldReturnCorrectMovieWithGenre()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var genreId = context.Genres.Select(x => x.Id).First();

        var filterDto = new FilterMoviesDto()
        {
            GenreId = genreId
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Movie with genre", movies[0].Title);
    }

    [TestMethod]
    public async Task FilterMovie_WithFieldOrderByTitle_ShouldOrderTitleAscending()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMoviesDto()
        {
            FieldOrder = "title",
            AscendingOrder = true,
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;

        var context2 = BuildContext(nameDb);
        var moviesDb = context2.Movies.OrderBy(x => x.Title).ToList();
        
        Assert.AreEqual(moviesDb.Count, movies.Count);

        for (int i = 0; i < moviesDb.Count; i++)
        {
            var movieFromController = movies[i];
            var movieDb = moviesDb[i];
            
            Assert.AreEqual(movieDb.Id, movieFromController.Id);
        }

    }
    
    
    [TestMethod]
    public async Task FilterMovie_WithFieldOrderByTitleDescending_ShouldOrderTitleDescending()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filterDto = new FilterMoviesDto()
        {
            FieldOrder = "title",
            AscendingOrder = false,
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;

        var context2 = BuildContext(nameDb);
        var moviesDb = context2.Movies.OrderByDescending(x => x.Title).ToList();
        
        Assert.AreEqual(moviesDb.Count, movies.Count);

        for (int i = 0; i < moviesDb.Count; i++)
        {
            var movieFromController = movies[i];
            var movieDb = moviesDb[i];
            
            Assert.AreEqual(movieDb.Id, movieFromController.Id);
        }

    }

    [TestMethod]
    public async Task Filter_InvalidField_ShouldReturnMovies()
    {
        var nameDb = DataTest();
        var mapper = ConfigurateAutoMapper();
        var context = BuildContext(nameDb);

        var mock = new Mock<ILogger<MoviesController>>();
        
        var controller = new MoviesController(context, mapper, null, mock.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var filterDto = new FilterMoviesDto()
        {
            FieldOrder = "abc",
            AscendingOrder = true
        };

        var response = await controller.GetMoviesByFilter(filterDto);
        var movies = response.Value;
        var context2 = BuildContext(nameDb);
        var moviesDb = context2.Movies.ToList();
        Assert.AreEqual(moviesDb.Count, movies.Count);
        Assert.AreEqual(1, mock.Invocations.Count);
    }
    
    private string DataTest()
    {
        var databaseName = Guid.NewGuid().ToString();
        var context = BuildContext(databaseName);
        var genre = new Genre() {Name = "Genre test"};

        var movies = new List<Movie>()
        {
            new Movie() { Title = "Movie 1", DatePremiere = new DateTime(2020,1,1), Showcasing = false},
            new Movie() { Title = "Movie 2 release tomorrow", DatePremiere = DateTime.Today.AddDays(1), Showcasing = false},
            new Movie() { Title = "Movie 3 on cinema", DatePremiere = DateTime.Today.AddDays(-1), Showcasing = true},
        };

        var movieWithGenre = new Movie()
        {
            Title = "Movie with genre",
            DatePremiere = new DateTime(2015,1,1),
            Showcasing = false
        };
        movies.Add(movieWithGenre);

        context.Add(genre);
        context.AddRange(movies);
        var movieGenre = new MoviesGenres() {GenreId = genre.Id, MovieId = movieWithGenre.Id};
        context.Add(movieGenre);
        context.SaveChanges();

        return databaseName;
    }
    

}