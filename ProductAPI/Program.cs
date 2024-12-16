using Microsoft.EntityFrameworkCore;
using ProductAPI.Extensions;
using ProductAPI.Models;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Discovery.Client;
using ProductAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//discovery
builder.Services.AddServiceDiscovery(options => options.UseEureka());
//builder.Services.AddHealthChecks();
//builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

// Database Context DI

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = "P@ssw0rd121#";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
// var connectionString = "Server=.; Database=ProductDb; Integrated Security=True";
builder.Services.AddDbContext<ProductDbContext>(opt => opt.UseSqlServer(connectionString));

//==============================

builder.AddAppAuthetication();

builder.Services.AddAuthorization();

builder.Services.AddHostedService<RabbitMQListener>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;
    ProductAPI.SeedData.Initialize(serviceProvider);
}

app.MapControllers();

//app.UseHealthChecks("/info");

app.Run();
