using Article.Core.Common;
using Article.Core.IReposiories;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Common;
using Article.Infrastructure.Repositories;
using Article.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Article.Infrastructure.ExtensionService
{
    public static class InfrastructureExtensionService
    {
        public static async Task<IServiceCollection> AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddDbContext<ArticleDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
                 sqlServerOptions =>
                 {
                     sqlServerOptions.EnableRetryOnFailure(
                         maxRetryCount: 5, // Number of retries
                         maxRetryDelay: TimeSpan.FromSeconds(30), // Delay between retries
                         errorNumbersToAdd: null // Optional error codes to retry on
                     );
                 }).EnableServiceProviderCaching());

            services.AddScoped<IDbFactory, DbFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBlogRepository, BlogRepository>();


            // Register all classes that implement IRepository with their interfaces
            var repositoryTypes = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IGenericRepositories<>))))
                .ToList();

            foreach (var type in repositoryTypes)
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(i => i != typeof(IGenericRepositories<>));
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, type);
                }
            }

            services.AddScoped<Seeder>();

            // Seed initial data using Seeder class
            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
                    await seeder.SeedData();
                }
            }
            return services;
        }

    }
}
