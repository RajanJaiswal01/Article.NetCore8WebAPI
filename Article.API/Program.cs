using Article.Application.Blog.Query.GetById;
using Article.Application.ExtensionService;
using Article.Application.Middleware;
using Article.Core.Entities;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.ExtensionService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Read configuration from appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console()  // Log to console
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)  // Log to file, daily rotation
    .CreateLogger();

// Add Serilog to the logging providers
builder.Host.UseSerilog();

//Adding IdentityEndPoinsts 
builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ArticleDbContext>()
    .AddDefaultTokenProviders();

//Adding Extension Services
await builder.Services.AddInfrastructureDI(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddApplicationDI();


//builder.Services.AddIdentity<User, IdentityRole>()
//       .AddEntityFrameworkStores<ArticleDbContext>()
//       .AddDefaultTokenProviders();



builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MediatR and specify the assemblies containing handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetByIdQueryHandler).GetTypeInfo().Assembly));

var app = builder.Build();

//configuring the identityapi
app.MapGroup("/api/Identity").MapIdentityApi<User>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Middleware that should run for every request
app.UseMiddleware<CustomMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
