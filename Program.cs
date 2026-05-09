using Microsoft.Extensions.Caching.Memory;
using TrackYourTasksAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register in-memory cache (already present if you use caching elsewhere)
builder.Services.AddMemoryCache();

// Register services
builder.Services.AddSingleton<MongoTaskService>();
builder.Services.AddSingleton<DailyTaskService>();

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
