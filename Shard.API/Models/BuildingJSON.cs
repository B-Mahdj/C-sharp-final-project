using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class BuildingJson
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        [JsonIgnore]
        public string? BuilderId { get; set; }
        public string? ResourceCategory { get; set; }
        public string? System { get; set; }
        public string? Planet { get; set; }
        public bool IsBuilt { get; set; }
        public DateTime? EstimatedBuildTime { get; set; }



        public BuildingJson(Building building)
        {
            this.Id = building.Id;
            this.Type = building.Type;
            this.BuilderId = building.BuilderId;
            this.ResourceCategory = building.ResourceCategory;
            this.System = building.System;
            this.Planet = building.Planet;
            this.IsBuilt = building.IsBuilt ?? false;
            this.EstimatedBuildTime = building.EstimatedBuildTime;
        }
    }
}
