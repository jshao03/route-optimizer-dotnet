using RouteOptimizer.Core.Configuration;
using RouteOptimizer.Core.Data;
using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteSettings>(
    builder.Configuration.GetSection("RouteSettings"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "routeoptimizer:";
});

builder.Services.AddSingleton(SampleGraphFactory.Create());

builder.Services.AddSingleton<DijkstraRouteFinder>();
builder.Services.AddSingleton<IRouteFinder, RedisCachedRouteFinder>();

var app = builder.Build();

app.UseCors("frontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();