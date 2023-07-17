using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Movies.Controllers;
using Movies.DTOs;
using Movies.Entities;

namespace MoviesTest.UnitTest;

[TestClass]
public class MoviesControllerTests: TestBase
{

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