using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration;
using System;
using UserAPI;
using UserAPI.Models;
using UserAPI.Service;
using UserAPI.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//discovery
builder.Services.AddServiceDiscovery(options => options.UseEureka());
//builder.Services.AddHealthChecks();
//builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

// Database Context DI
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
Console.WriteLine(dbHost);
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = "P@ssw0rd121#";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
// var connectionString = "Server=.; Database=UserDb; Integrated Security=True";
builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(connectionString));
//==============================

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
    SeedData.InitializeAsync(serviceProvider).Wait();
}

app.MapControllers();

//app.UseHealthChecks("/info");

app.Run();
