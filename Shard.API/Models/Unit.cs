using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Unit
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
       


        [JsonConstructor]
        public Unit(string id, string type, string system, string planet)
        {
            this.Id = id;
            this.Type = type;
            this.System = system;
            this.Planet = planet;

        }
        public Unit(string id, string type, string system)
        {
            this.Id = id;
            this.Type = type;
            this.System = system;
            this.Planet = null;
            

        }
    }
}
