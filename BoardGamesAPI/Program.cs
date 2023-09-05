using System.Text.Json;
using BoardGamesAPI;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

var app = builder.Build();

var memoryCache = app.Services.GetRequiredService<IMemoryCache>();

if(!memoryCache.TryGetValue("boardGamesList", out List<BoardGames> boardGamesList))
{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "board-games-ranking.json");
    string jsonContent = File.ReadAllText(filePath);
    boardGamesList = JsonSerializer.Deserialize<List<BoardGames>>(jsonContent);

    var cacheEntryOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
    };

    // Armazene os dados em cache
    memoryCache.Set("boardGamesList", boardGamesList, cacheEntryOptions);
}


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
