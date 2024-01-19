using SayList.Engines;
using SayList.Interfaces;
using SayList.Managers;
using SayList.Utilities;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register SpotifyAuthorizationService
builder.Services.AddScoped<IAuthorizationEngine, SpotifyAuthorizationEngine>();
builder.Services.AddScoped<IPlaylistGeneratorEngine, SpotifyPlaylistGeneratorEngine>();


builder.Services.AddScoped<IPlaylistGeneratorManager, SpotifyPlayListGeneratorManager>();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient();
builder.Services.AddTransient<HttpRequestUtility>();

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddUserSecrets("8710ec59 - 9037 - 4aca - 963a - a8a4840894e0");

var app = builder.Build();

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
