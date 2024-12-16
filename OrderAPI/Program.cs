using OrderAPI.Extensions;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Discovery.Client;
using OrderAPI.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//discovery
builder.Services.AddServiceDiscovery(options => options.UseEureka());
//builder.Services.AddHealthChecks();
//builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

//MongoDb configuration
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var connectionString = $"mongodb://{dbHost}:27017/{dbName}";
//var connectionString = $"mongodb://localhost:27017/orderdb";

var mongoUrl = MongoUrl.Create(connectionString);

// Register MongoDB client
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
    new MongoClient(mongoUrl));

// Register MongoDB database
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
    serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(mongoUrl.DatabaseName));


builder.AddAppAuthetication();

builder.Services.AddAuthorization();

builder.Services.AddHostedService<RabbitMQListener>();

builder.Services.AddScoped<IMessageProducer, MessageProducer>();

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

app.MapControllers();

//app.UseHealthChecks("/info");

app.Run();
