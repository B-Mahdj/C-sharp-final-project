using Shard.Shared.Core;
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
        [JsonIgnore]
        public int Damage { get; set; }
        [JsonIgnore]
        public int ReloadTime { get; set; }
        public int Health { get; set; }
        public Dictionary<ResourceKind, int>? ResourcesQuantity { get; set; }


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
            Damage = unit.Damage;
            ReloadTime = unit.ReloadTime;
            Health = unit.Health;
            ResourcesQuantity = unit.ResourcesQuantity;
        }
    }
}
