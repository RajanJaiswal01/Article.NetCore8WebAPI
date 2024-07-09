using Article.Application.MapData;
using Article.Application.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Article.Application.ExtensionService
{
    public static class ApplicationExtensionService
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // Register CustomMiddleware in the DI container
            services.AddTransient<CustomMiddleware>();

            // Register AutoMapper and the assemblies containing mapping profiles
            services.AddAutoMapper(typeof(MappingConfig).Assembly);

            return services;
        }
    }
}
