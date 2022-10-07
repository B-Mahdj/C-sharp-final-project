using Shard.API.Models;
using Shard.Shared.Core;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MapGenerator>();
builder.Services.AddSingleton<Sector>();
builder.Services.AddSingleton(new List<User>());
builder.Services.AddSingleton(new List<Sector>());

builder.Configuration.GetSection("MapGeneratorOptions");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
MapGenerator? map = app.Services.GetService<MapGenerator>();
Sector? sectorFinale = app.Services.GetService<Sector>();
if (sectorFinale != null)
    if (map != null)
    sectorFinale.Generate(map);

app.Services.GetService<List<User>>();
app.Run();



namespace Shard.API
{
    public partial class Program { }
}
