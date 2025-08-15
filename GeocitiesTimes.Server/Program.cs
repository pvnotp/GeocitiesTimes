using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers;
using GeocitiesTimes.Server.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddScoped<INewsClient, NewsClient>();
builder.Services.AddScoped<IArticleProvider, ArticleProvider>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMemoryCache();

builder.Services.Configure<MemoryCacheOptions>(options =>
{
    options.TrackStatistics = true;
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
