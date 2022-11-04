using System.Text.Json.Serialization;

namespace Shard.API.Models
{
    public class Mine : Building
    {
        public string? ResourceCategory { get; set; } 
        public Mine(string type, string builderId, string resourceCategory) : base(type, builderId) 
        {
            this.ResourceCategory = resourceCategory;
        }
    }
}
