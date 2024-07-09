using Article.Core.Entities;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

public class SeederTests
{
    private ArticleDbContext CreateContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ArticleDbContext(options);
        context.Database.EnsureDeleted(); // Ensure database is deleted
        context.Database.EnsureCreated(); // Ensure database is created

        return context;
    }

    [Fact]
    public async Task SeedData_ShouldSeedUsersAndData_WhenDatabaseIsEmpty()
    {
        // Arrange
        var dbContext = CreateContext();

        var serviceProviderMock = new Mock<IServiceProvider>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<Seeder>>();

        // Configure mock objects
        configurationMock.Setup(c => c["AppSettings:SuperAdminEmail"]).Returns("admin@example.com");
        configurationMock.Setup(c => c["AppSettings:SuperAdminPassword"]).Returns("Password123!");

        var userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        serviceProviderMock.Setup(sp => sp.GetService(typeof(UserManager<User>)))
            .Returns(userManagerMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
            .Returns(roleManagerMock.Object);

        // Setup userManager and roleManager behavior
        userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        roleManagerMock.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);

        var seeder = new Seeder(dbContext, serviceProviderMock.Object, configurationMock.Object, loggerMock.Object);

        // Act
        await seeder.SeedData();

        // Assert
        Assert.True(dbContext.Users.Any());
        Assert.True(dbContext.Author.Any());
        Assert.True(dbContext.Blog.Any());
    }
}
