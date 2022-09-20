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
var map = app.Services.GetService<MapGenerator>();
var sectorFinale = app.Services.GetService<Sector>();
sectorFinale.Generate(map);
app.Run();



namespace Shard.API
{
    public partial class Program { }
}
