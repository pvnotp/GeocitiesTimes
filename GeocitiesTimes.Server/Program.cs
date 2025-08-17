using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Providers.Pages;
using GeocitiesTimes.Server.Providers.Stories;
using GeocitiesTimes.Server.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.Configure<MemoryCacheOptions>(options =>
{
    options.SizeLimit = 500; //Chosen as it is the maximum number of new stories.
});


builder.Services.AddScoped<INewsClient, NewsClient>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IStoriesProvider, StoriesProvider>();
builder.Services.AddScoped<IPagesProvider, PagesProvider>();

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
