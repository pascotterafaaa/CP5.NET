using JogosApi.Repositories;
using JogosApi.Services;
using JogosApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Options Pattern: appsettings.json -> MongoDbSettings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(MongoDbSettings.SectionName));

// IMongoClient / IMongoDatabase (singletons, thread-safe)
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return sp.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName);
});

// Camadas: Controller > JogoService > IJogoRepository
builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IJogoService, JogoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Catálogo de Jogos API");
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
});

app.MapControllers();

app.Run();
