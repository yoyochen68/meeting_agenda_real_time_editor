using MeetingAgenda.Models;
using Microsoft.EntityFrameworkCore;
using MeetingAgenda.Controllers;
using Microsoft.AspNetCore.OpenApi;
using MeetingAgenda.Hubs;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

builder.Services.AddDbContext<DatabaseContext>(
    opt =>
    {
        opt.UseNpgsql(connectionString);
        if (builder.Environment.IsDevelopment())
        {
            opt
              .LogTo(Console.WriteLine, LogLevel.Information)
              .EnableSensitiveDataLogging()
              .EnableDetailedErrors();
        }
    }
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();



var app = builder.Build();
app.MapControllers();
app.MapHub<MeetingHub>("/r/meeting");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// http://localhost:5175/swagger/index.html

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");


app.Run();


