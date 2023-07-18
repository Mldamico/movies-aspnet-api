using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Movies.Controllers;
using Movies.DTOs;

namespace MoviesTest.UnitTest;

[TestClass]
public class AccountsControllerTests: TestBase
{

    [TestMethod]
    public async Task Signup_ShouldCreateUser()
    {
        var nameDb = Guid.NewGuid().ToString();
        await CreateUser(nameDb);
        var context2 = BuildContext(nameDb);
        var userCount = await context2.Users.CountAsync();
        Assert.AreEqual(1, userCount);
    }

    [TestMethod]
    public async Task Login_WithWrongCredentials_ShouldThrowError()
    {
        var nameDb = Guid.NewGuid().ToString();
        await CreateUser(nameDb);
        var controller = BuildAccountController(nameDb);
        var userInfo = new UserInfo()
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };
        var response = await controller.Login(userInfo);
        Assert.IsNull(response.Value);
        var result = response.Result as BadRequestObjectResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task Login_WithCorrectCredentials_IsAbleToLoginSuccessfully()
    {
        var nameDb = Guid.NewGuid().ToString();
        await CreateUser(nameDb);
        var controller = BuildAccountController(nameDb);
        var userInfo = new UserInfo()
        {
            Email = "test@test.com",
            Password = "Pa$$w0rd"
        };
        var response = await controller.Login(userInfo);
        Assert.IsNotNull(response.Value);
        
    }
    
    private async Task CreateUser(string nameDb)
    {
        var accountController = BuildAccountController(nameDb);
        var userInfo = new UserInfo()
        {
            Email = "test@test.com",
            Password = "Pa$$w0rd"
        };

        await accountController.CreateUser(userInfo);
    }

    private AccountController BuildAccountController(string nameDb)
    {
        var context = BuildContext(nameDb);
        var userStore = new UserStore<IdentityUser>(context);
        var userManager = BuildUserManager(userStore);
        var mapper = ConfigurateAutoMapper();
        var httpContext = new DefaultHttpContext();
        MockAuth(httpContext);
        var signingManager = SetupSignInManager(userManager, httpContext);
        var configurationDictionary = new Dictionary<string, string>()
        {
            {"jwt:key", "ASKDANMKSFDNAKFAKFNAKFNASFKANFAK"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationDictionary)
            .Build();
        return new AccountController(userManager, signingManager, configuration, context, mapper);
    }
    
    public static UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
    {
        store = store ?? new Mock<IUserStore<TUser>>().Object;
        var options = new Mock<IOptions<IdentityOptions>>();
        var idOptions = new IdentityOptions();
        idOptions.Lockout.AllowedForNewUsers = false;
        options.Setup(o => o.Value).Returns(idOptions);
        var userValidators = new List<IUserValidator<TUser>>();
        var validator = new Mock<IUserValidator<TUser>>();
        userValidators.Add(validator.Object);
        var pwdValidators = new List<PasswordValidator<TUser>>();
        pwdValidators.Add(new PasswordValidator<TUser>());
        var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
            userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null,
            new Mock<ILogger<UserManager<TUser>>>().Object);
        validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
        return userManager;
    }
    
    private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
        HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
        IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(a => a.HttpContext).Returns(context);
        identityOptions = identityOptions ?? new IdentityOptions();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(a => a.Value).Returns(identityOptions);
        var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
        schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
        var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
        sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
        return sm;
    }
    
    private Mock<IAuthenticationService> MockAuth(HttpContext context)
    {
        var auth = new Mock<IAuthenticationService>();
        context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
        return auth;
    }
}