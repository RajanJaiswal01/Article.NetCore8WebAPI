using Article.Core.Entities;
using Article.Infrastructure.ApplicationDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Article.Infrastructure.Seeds
{
    public class Seeder
    {
        private readonly ArticleDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Seeder> _logger;

        public Seeder(ArticleDbContext dbContext, IServiceProvider serviceProvider, IConfiguration configuration, ILogger<Seeder> logger)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedData()
        {
            try
            {
                await _dbContext.Database.MigrateAsync();
                // Ensure database can connect and apply migrations
                if (await _dbContext.Database.CanConnectAsync())
                {
                    if (_dbContext.Users.Any() && _dbContext.Author.Any() && _dbContext.Blog.Any()) return;
                    //var strategy = _dbContext.Database.CreateExecutionStrategy();

                   // strategy.Execute(async () =>
                   //{
                   //    using var transaction = await _dbContext.Database.BeginTransactionAsync();

                       try
                       {

                           // Seed users if none exist
                           if (!_dbContext.Users.Any())
                           {
                               await InitializeUser();
                           }

                           // Check if super admin exists and seed related data
                           var superAdminEmail = _configuration["AppSettings:SuperAdminEmail"];
                           var userData = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == superAdminEmail);
                           var userId = userData?.Id;


                           if (!string.IsNullOrEmpty(userId))
                           {
                               if (!_dbContext.Author.Any())
                               {
                                   Author author = new Author()
                                   {
                                       UserId = userId,
                                       Category = "Cultural Author",
                                       Designation = "Senior Author",
                                       CreatedBy = userId,
                                       CreatedOn = DateTime.UtcNow,
                                       IsActive = true,
                                       IsDeleted = false
                                   };
                                   await _dbContext.Author.AddAsync(author);
                                   await _dbContext.SaveChangesAsync();
                               }

                               if (_dbContext.Author.Any())
                               {
                                   if (!_dbContext.Blog.Any())
                                   {
                                       var author = await _dbContext.Author.FirstOrDefaultAsync(x => x.UserId == userId);
                                       var BlogsData = SeedBlogData(userId, author.Id);
                                       if (BlogsData.Count() > 0)
                                       {
                                           await _dbContext.Blog.AddRangeAsync(BlogsData);
                                       }
                                   }
                               }
                           }

                           await _dbContext.SaveChangesAsync();
                           //await transaction.CommitAsync();
                       }
                       catch (Exception ex)
                       {
                           //await transaction.RollbackAsync();
                           // Log the exception
                           Console.WriteLine($"Error occurred during data seeding: {ex.Message}");
                           _logger.LogError($"Error occurred during data seeding: {ex.Message}");
                           throw; // Rethrow the exception for higher-level handling
                       }
                  // });
                }
                else
                {
                    Console.WriteLine($"Cannot able to established connection to database");
                    _logger.LogError($"Cannot able to established connection to database");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                _logger.LogError($"Error connecting to database: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
            }
        }

        private async Task InitializeUser()
        {
            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure the roles are created
            string[] roleNames = { "Admin", "Author", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        throw new ApplicationException($"Error creating role: {roleName}. Check inner exception for details.");
                    }
                }
            }

            // Create a super admin user if not exists
            var superAdminEmail = _configuration["AppSettings:SuperAdminEmail"];
            var superAdminPassword = _configuration["AppSettings:SuperAdminPassword"];

            if (superAdminEmail != null && superAdminPassword != null)
            {
                var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
                if (superAdmin == null)
                {
                    superAdmin = new User()
                    {
                        UserName = superAdminEmail,
                        Email = superAdminEmail,
                        PhoneNumber = "9807654321",
                        Nationality = "Nepali",
                        Address = new Address()
                        {
                            Country = "Nepal",
                            State = "Madesh Pradesh",
                            Street = "Gaushala",
                            PostalCode = "7863473",
                        }
                        // Add more properties as needed
                    };

                    var result = await userManager.CreateAsync(superAdmin, superAdminPassword);
                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"Error creating super admin user. Check inner exception for details.");
                    }

                    // Assign the super admin role
                    await userManager.AddToRoleAsync(superAdmin, "Admin");
                }
            }
        }

        private IEnumerable<Blog> SeedBlogData(string userId, long authorId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var blogsData = new List<Blog>()
                {
                    new Blog()
                    {
                        Name = "Learn C#",
                        Description = "C# from Zero to Hero.",
                        IsDeleted = false,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = userId,
                        Posts = new List<Post>()
                        {
                            new Post()
                            {
                                Title = "Basic of C#",
                                Description = "C# is an object-oriented programming language.",
                                AuthorId = authorId, // Replace with the correct author ID based on your logic
                                CreatedBy = userId,
                                IsDeleted = false,
                                IsActive = true,
                                CreatedOn = DateTime.UtcNow,
                            },
                            new Post()
                            {
                                Title = "Fundamentals of C#",
                                Description = "C# has two main data types.",
                                AuthorId = authorId, // Replace with the correct author ID based on your logic
                                CreatedBy = userId,
                                IsDeleted = false,
                                IsActive = true,
                                CreatedOn = DateTime.UtcNow,
                            }
                        }
                    }
                };

                return blogsData;
            }
            return Enumerable.Empty<Blog>();
        }
    }
}
