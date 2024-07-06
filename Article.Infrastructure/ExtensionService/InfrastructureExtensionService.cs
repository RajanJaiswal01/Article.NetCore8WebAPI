using Article.Core.Common;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Common;
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

            var types = assembly.GetTypes()
           .Where(t => t.IsClass && !t.IsAbstract)
           .Select(t => new
           {
               Interface = t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}"),
               Implementation = t
           })
           .Where(t => t.Interface != null);

            foreach (var type in types)
            {
                services.AddTransient(type.Interface, type.Implementation);
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
