using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Providers.Pages;
using GeocitiesTimes.Server.Providers.Stories;
using GeocitiesTimes.Server.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
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

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options => 
{
    options.AddPolicy("CORSPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CORSPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
