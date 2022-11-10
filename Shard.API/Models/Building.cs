using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Building
    {
        public string? Id { get; set; }
        public string Type { get; set; }
        public string? BuilderId { get; set; }
        public string? ResourceCategory { get; set; }
        [JsonIgnore]
        public string? System { get; set; }
        [JsonIgnore]
        public string? Planet { get; set; }
        public bool? IsBuilt { get; set; } = false;
        public DateTime? EstimatedBuildTime { get; set; }

        public Task? BuildingTask { get; set; }


        public Building(string type, string builderId, string resourceCategory)
        {
            this.Type = type;
            this.BuilderId = builderId;
            this.ResourceCategory = resourceCategory;
        }

    }
}
