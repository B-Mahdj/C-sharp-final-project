using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class UnitJson
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string System { get; set; }
        public string? Planet { get; set; }
        public string? DestinationSystem { get; set; }
        public string? DestinationPlanet { get; set; }
        public string? EstimatedTimeOfArrival { get; set; }
        [JsonIgnore]
        public Task? MovingTask { get; set; }
        [JsonIgnore]
        public Task? BuildingTask { get; set; }


        public UnitJson(Unit unit)
        {
            Id = unit.Id;
            Type = unit.Type;
            System = unit.System;
            Planet = unit.Planet;
            DestinationSystem = unit.DestinationSystem;
            DestinationPlanet = unit.DestinationPlanet;
            EstimatedTimeOfArrival = unit.EstimatedTimeOfArrival;
            MovingTask = unit.MovingTask;
        }
    }
}
