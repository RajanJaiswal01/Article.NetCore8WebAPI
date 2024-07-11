using Article.Core.Entities;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Article.Infrastructure.Tests
{
    public class SeederTests
    {
        private readonly Mock<ArticleDbContext> _mockDbContext;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<Seeder>> _mockLogger;

        public SeederTests()
        {
            // Mock DbContext and DbSet
            _mockDbContext = new Mock<ArticleDbContext>(new DbContextOptions<ArticleDbContext>());
            // Set up DbSet mocks
            _mockDbContext.Setup(d => d.Set<User>()).Returns(MockDbSet<User>(new List<User>()));
            _mockDbContext.Setup(d => d.Set<Author>()).Returns(MockDbSet<Author>(new List<Author>()));
            _mockDbContext.Setup(d => d.Set<Blog>()).Returns(MockDbSet<Blog>(new List<Blog>()));

            // Mock ServiceProvider
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(ArticleDbContext)))
                .Returns(_mockDbContext.Object);

            // Mock IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock ILogger
            _mockLogger = new Mock<ILogger<Seeder>>();
        }

        [Fact]
        public async Task SeedData_NoExistingData_Succeeds()
        {
            // Arrange
            var seeder = new Seeder(
                _mockDbContext.Object,
                _mockServiceProvider.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            // Mock configuration values
            _mockConfiguration.SetupGet(c => c["AppSettings:SuperAdminEmail"]).Returns("admin@example.com");

            // Mock DbContext methods
            _mockDbContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1); // Mock SaveChangesAsync
            _mockDbContext.Setup(d => d.Set<User>().AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockDbContext.Setup(d => d.Set<Author>().AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockDbContext.Setup(d => d.Set<Blog>().AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            await seeder.SeedData();

            // Assert
            _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _mockDbContext.Verify(d => d.Set<User>().AddRangeAsync(It.IsAny<IEnumerable<User>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _mockDbContext.Verify(d => d.Set<Author>().AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _mockDbContext.Verify(d => d.Set<Blog>().AddRangeAsync(It.IsAny<IEnumerable<Blog>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task SeedData_ExistingData_SkipsSeeding()
        {
            // Arrange
            var seeder = new Seeder(
                _mockDbContext.Object,
                _mockServiceProvider.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            // Mock DbContext methods
            _mockDbContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0); // Mock SaveChangesAsync
            _mockDbContext.Setup(d => d.Set<User>().AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockDbContext.Setup(d => d.Set<Author>().AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockDbContext.Setup(d => d.Set<Blog>().AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            await seeder.SeedData();

            // Assert
            _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockDbContext.Verify(d => d.Set<User>().AddRangeAsync(It.IsAny<IEnumerable<User>>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockDbContext.Verify(d => d.Set<Author>().AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockDbContext.Verify(d => d.Set<Blog>().AddRangeAsync(It.IsAny<IEnumerable<Blog>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Helper method to mock DbSet<T>
        private static DbSet<T> MockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockDbSet.Setup(d => d.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>())).ReturnsAsync((T entity, CancellationToken token) =>
            {
                data = data.Append(entity);
                return new EntityEntry<T>(default);
            });
            mockDbSet.Setup(d => d.AnyAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate, CancellationToken token) => data.AsQueryable().Any(predicate));
            return mockDbSet.Object;
        }
    }
}
