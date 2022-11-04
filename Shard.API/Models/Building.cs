using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Building
    {
        public string? Id { get; set; }
        public string Type { get; set; }
        public string? BuilderId { get; set; }
        [JsonIgnore]
        public string? System { get; set; }
        [JsonIgnore]
        public string? Planet { get; set; }

        public Building(string type, string builderId)
        {
            this.Type = type;
            this.BuilderId = builderId;   
        }

    }
}
