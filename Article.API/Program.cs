using Article.Core.Entities;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.ExtensionService;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Adding IdentityEndPoinsts 
builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ArticleDbContext>()
    .AddDefaultTokenProviders();

//Adding Extension Services
builder.Services.AddInfrastructureDI(builder.Configuration, Assembly.GetExecutingAssembly());

//builder.Services.AddIdentity<User, IdentityRole>()
//       .AddEntityFrameworkStores<ArticleDbContext>()
//       .AddDefaultTokenProviders();



builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//configuring the identityapi
app.MapGroup("/Identity").MapIdentityApi<User>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
